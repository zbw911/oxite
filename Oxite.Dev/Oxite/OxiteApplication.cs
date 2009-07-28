//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Oxite.BootStrapperTasks;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Plugins;
using Oxite.Repositories;
using Oxite.Repositories.SqlServer;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite
{
    public class OxiteApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Application["container"] = setupContainer();
            Application["bootStrappersLoaded"] = false;

            load();
        }

        protected void Application_End()
        {
            unload();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Site site = ((IUnityContainer)Application["container"]).Resolve<ISiteService>().GetSite();

            if (site == null)
            {
                string setupUrl = new UrlHelper(new RequestContext(new HttpContextWrapper(Context), new RouteData())).Site();

                if (!Request.RawUrl.EndsWith(setupUrl, StringComparison.OrdinalIgnoreCase) && Request.RawUrl.IndexOf("/skins", StringComparison.OrdinalIgnoreCase) == -1 && !Request.RawUrl.StartsWith("/Content", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect(setupUrl, true);
                }
            }
            
            if (site.ID != Guid.Empty && !hasSameHostAsRequest(site.Host))
            {
                Uri hostAlias = site.HostRedirects.Where(hasSameHostAsRequest).FirstOrDefault();

                if (hostAlias != null)
                {
                    Response.RedirectLocation = makeHostMatchRequest(site.Host).ToString();
                    Response.StatusCode = 301;
                    Response.End();
                }
            }

            //TODO: (erikpo) Need to find a better way for legacy apps to get access to the original HttpRequest and HttpResponse instead of HttpRequestBase and HttpResponseBase
            Context.Items["originalRequest"] = Context.Request;
            Context.Items["originalResponse"] = Context.Response;
        }

        public static void Load(HttpContextBase context)
        {
            IEnumerable<IBootStrapperTask> tasks = ((IUnityContainer)context.Application["container"]).ResolveAll<IBootStrapperTask>();
            bool bootStrappersLoaded = (bool)context.Application["bootStrappersLoaded"];
            IDictionary<string, object> state = (IDictionary<string, object>)context.Application["bootStrapperState"];

            if (state == null)
                context.Application["bootStrapperState"] = state = new Dictionary<string, object>();

            if (bootStrappersLoaded)
                foreach (IBootStrapperTask task in tasks)
                    task.Cleanup(state);

            foreach (IBootStrapperTask task in tasks)
                task.Execute(state);

            context.Application["bootStrappersLoaded"] = true;
        }

        private void load()
        {
            Load(new HttpContextWrapper(Context));
        }

        private void unload()
        {
            IDictionary<string, object> state = (IDictionary<string, object>)Context.Application["bootStrapperState"];

            foreach (IBootStrapperTask task in ((IUnityContainer)Context.Application["container"]).ResolveAll<IBootStrapperTask>())
                task.Cleanup(state);
        }

        private bool hasSameHostAsRequest(Uri url)
        {
            if (!string.Equals(url.Scheme, Request.Url.Scheme, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.Equals(url.Host, Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                return false;

            if (url.Port != Request.Url.Port)
                return false;

            return true;
        }

        private Uri makeHostMatchRequest(Uri url)
        {
            if (url == null) return null;

            UriBuilder builder = new UriBuilder(Request.Url);
            UriBuilder builder2 = new UriBuilder(url);

            builder.Scheme = builder2.Scheme;
            builder.Host = builder2.Host;
            builder.Port = builder2.Port;

            return builder.Uri;
        }

        private IUnityContainer setupContainer()
        {
            IUnityContainer parentContainer = new UnityContainer();

            parentContainer
                .RegisterInstance((OxiteConfigurationSection)ConfigurationManager.GetSection("oxite"))
                .RegisterInstance(new AppSettingsHelper(ConfigurationManager.AppSettings))
                .RegisterInstance(RouteTable.Routes)
                .RegisterInstance(ModelBinders.Binders)
                .RegisterInstance(ViewEngines.Engines)
                .RegisterInstance(HostingEnvironment.VirtualPathProvider);

            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
                parentContainer.RegisterInstance(connectionString.Name, connectionString.ConnectionString);

            parentContainer
                .RegisterInstance<IBootStrapperTask>("LoadModules", new LoadModules(parentContainer));

            parentContainer
                .RegisterType<IModuleService, ModuleService>()
                .RegisterType<IModulesLoaded, ModulesLoaded>()
                .RegisterType<IPluginEngine, PluginEngine>()
                .RegisterType<ISiteService, SiteService>()
                .RegisterType<IValidationService, ValidationService>();

            //TODO: (erikpo) Add a check here for if the app is running on sql or xml files
            parentContainer
                .RegisterType<OxiteDataContext>(new InjectionConstructor(new ResolvedParameter<string>("ApplicationServices")))
                .RegisterType<IModuleRepository, SqlServerModuleRepository>()
                .RegisterType<ISiteRepository, SqlServerSiteRepository>()
                .RegisterType<ILocalizationRepository, SqlServerLocalizationRepository>()
                .RegisterType<ILanguageRepository, SqlServerLanguageRepository>()
                .RegisterType<IMessageOutboundRepository, SqlServerMessageOutboundRepository>()
                .RegisterType<IExtendedPropertyRepository, SqlServerExtendedPropertyRepository>();

            IUnityContainer oxiteContainer = parentContainer.CreateChildContainer();

            UnityConfigurationSection config = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            if (config.Containers.Default != null)
                config.Containers.Default.Configure(oxiteContainer);

            oxiteContainer.RegisterInstance(oxiteContainer);

            return oxiteContainer;
        }
    }
}
