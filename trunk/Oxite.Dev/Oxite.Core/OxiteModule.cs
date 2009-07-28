﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Oxite.Filters;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Core.Controllers;
using Oxite.Modules.Core.ModelBinders;
using Oxite.Modules.Membership.Filters;
using Oxite.Plugins;
using Oxite.Routing;
using Oxite.Services;
using Oxite.Skinning;
using Oxite.Validation;

namespace Oxite.Modules.Core
{
    public class OxiteModule : IOxiteModule
    {
        private readonly IUnityContainer container;

        public OxiteModule(IUnityContainer container)
        {
            this.container = container;
        }

        #region IOxiteModule Members

        public void Initialize()
        {
            setupControllerFactory();
            registerViewEngines();
            registerSkinResolvers();
            //loadBackgroundServices();
        }

        public void Unload()
        {
            //unloadBackgroundServices();
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            string[] controllerNamespaces = new string[] { "Oxite.Modules.Core.Controllers" };

            // Admin
            routes.MapRoute(
                "ManageSite",
                "Admin#manageSite",
                new { controller = "Site", action = "Dashboard" },
                new { routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "ManageBlogs",
                "Admin#manageBlogs",
                new { controller = "Site", action = "Dashboard" },
                new { routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "ManageUsers",
                "Admin#manageUsers",
                new { controller = "Site", action = "Dashboard" },
                new { routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "Site",
                "Admin/Setup",
                new { controller = "Site", action = "Item", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "Admin",
                "Admin",
                new { controller = "Site", action = "Dashboard", role = "Admin" },
                null,
                controllerNamespaces
                );

            // Base

            routes.MapRoute(
                "OpenSearch",
                "OpenSearch.xml",
                new { controller = "Utility", action = "OpenSearch" },
                new { includeOpenSearch = new OpenSearchConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "OpenSearchOSDX",
                "OpenSearch.osdx",
                new { controller = "Utility", action = "OpenSearchOSDX" },
                new { includeOpenSearch = new OpenSearchConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "ComputeHash",
                "ComputeHash",
                new { controller = "Utility", action = "ComputeHash" },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "GetDateTime",
                "GetDateTime",
                new { controller = "Utility", action = "GetDateTime" },
                new { httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "UserRoles",
                "Admin/Users/{userName}/Roles",
                new { controller = "User", action = "Roles", role = "Admin", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "RobotsTxt",
                "robots.txt",
                new { controller = "Utility", action = "RobotsTxt" },
                null,
                controllerNamespaces
                );
        }

        public void RegisterCatchAllRoutes(RouteCollection routes)
        {
        }

        public void RegisterFilters(IFilterRegistry filterRegistry)
        {
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(EnsureModelExceptionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(DebugActionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(SiteActionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(LocalizationActionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(ValidationLocalizationActionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(AntiForgeryAuthorizationFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(BlogSkinLayerResultFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(DialogActionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(ViewEnginesResultFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(ErrorExceptionFilter));
            filterRegistry.Add(Enumerable.Empty<IFilterCriteria>(), typeof(ResponseInsertResultFilter));

            filterRegistry.Add(new[] { new DataFormatFilterCriteria("RSS") }, typeof(RssResultActionFilter));
            filterRegistry.Add(new[] { new DataFormatFilterCriteria("ATOM") }, typeof(AtomResultActionFilter));
            filterRegistry.Add(new[] { new DataFormatFilterCriteria("JSON") }, typeof(JsonResultActionFilter));

            ControllerActionFilterCriteria adminActionsCriteria = new ControllerActionFilterCriteria();
            adminActionsCriteria.AddMethod<SiteController>(s => s.Dashboard());
            adminActionsCriteria.AddMethod<SiteController>(s => s.Item());
            filterRegistry.Add(new[] { adminActionsCriteria }, typeof(AuthorizationFilter));
        }

        public void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            modelBinders[typeof(ArchiveData)] = new ArchiveDataModelBinder();
            modelBinders[typeof(Site)] = new SiteModelBinder();
            modelBinders[typeof(OneMonthDateRangeAddress)] = new OneMonthDateRangeAddressModelBinder();
            modelBinders[typeof(DialogSelection)] = new DialogSelectionModelBinder();
        }

        public void RegisterWithContainer()
        {
            container
                .RegisterType<IActionInvoker, OxiteControllerActionInvoker>()
                .RegisterType<IViewEngine, OxiteWebFormViewEngine>()
                .RegisterType<IExtendedPropertyService, ExtendedPropertyService>()
                .RegisterType<IRegularExpressions, RegularExpressions>()
                //.RegisterType<IRegisterModules, OxiteRegisterModules>()
                .RegisterType<IRouteModifier, OxiteRouteUrlModifier>()
                .RegisterType<ISkinResolverRegistry, SkinResolverRegistry>();

            container
                .RegisterType<ILocalizationService, LocalizationService>()
                .RegisterType<ILanguageService, LanguageService>()
                .RegisterType<IMessageOutboundService, MessageOutboundService>()
                .RegisterType<IValidator<Site>, SiteValidator>();
        }

        #endregion

        #region Private Methods

        private void setupControllerFactory()
        {
            ControllerBuilder.Current.SetControllerFactory(container.Resolve<OxiteControllerFactory>());
        }

        private void registerViewEngines()
        {
            ViewEngineCollection viewEngines = container.Resolve<ViewEngineCollection>();

            viewEngines.Clear();

            viewEngines.Add(container.Resolve<OxiteWebFormViewEngine>());
        }

        private void registerSkinResolvers()
        {
            ISkinResolverRegistry skinResolverRegistry = container.Resolve<SkinResolverRegistry>();

            skinResolverRegistry.Default = container.Resolve<OxiteSkinResolver>();

            skinResolverRegistry.Add(container.Resolve<MobileSkinResolver>());
            skinResolverRegistry.Add(container.Resolve<LegacySkinResolver>());

            container.RegisterInstance(skinResolverRegistry);
        }

        //private void loadBackgroundServices()
        //{
        //    Site site = container.Resolve<ISiteService>().GetSite();

        //    if (site.ID != Guid.Empty)
        //    {
        //        BackgroundServicesExecutor backgroundServicesExecutor = (BackgroundServicesExecutor)(state.ContainsKey("backgroundServicesExecutor") ? state["backgroundServicesExecutor"] : null);

        //        if (backgroundServicesExecutor == null)
        //        {
        //            state["backgroundServicesExecutor"] = backgroundServicesExecutor = new BackgroundServicesExecutor(container);

        //            backgroundServicesExecutor.Start();
        //        }
        //    }
        //}

        //private void unloadBackgroundServices()
        //{
        //    BackgroundServicesExecutor backgroundServicesExecutor = (BackgroundServicesExecutor)(state.ContainsKey("backgroundServicesExecutor") ? state["backgroundServicesExecutor"] : null);

        //    if (backgroundServicesExecutor != null)
        //        backgroundServicesExecutor.Stop();
        //}

        #endregion
    }
}
