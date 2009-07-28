//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Oxite.Infrastructure;
using Oxite.Infrastructure.XmlRpc;
using Oxite.Modules.Blogs.Routing;
using Oxite.Modules.MetaWeblog.Repositories;
using Oxite.Modules.MetaWeblog.Repositories.SqlServer;
using Oxite.Modules.MetaWeblog.Services;

namespace Oxite.Modules.MetaWeblog
{
    public class MetaWeblogModule : IOxiteModule
    {
        private readonly IUnityContainer container;

        public MetaWeblogModule(IUnityContainer container)
        {
            this.container = container;
        }

        #region IOxiteModule Members

        public void Initialize()
        {
        }

        public void Unload()
        {
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            string[] controllerNamespaces = new string[] { "Oxite.Modules.MetaWeblog.Controllers" };

            routes.MapRoute(
                "Rsd",
                "RSD",
                new { controller = "MetaWeblog", action = "Rsd" },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "BlogRsd",
                "{blogName}/RSD",
                new { controller = "MetaWeblog", action = "Rsd" },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "LiveWriterManifest",
                "LiveWriterManifest.xml",
                new { controller = "MetaWeblog", action = "LiveWriterManifest" },
                null,
                controllerNamespaces
                );

            routes.Add(
                "MetaWeblog",
                new Route(
                    "MetaWeblog",
                    new RouteValueDictionary(new Dictionary<string, object> { { "controller", "MetaWeblog" } }),
                    new XmlRpcRouteHandler()
                    )
                );
        }

        public void RegisterCatchAllRoutes(RouteCollection routes)
        {
        }

        public void RegisterFilters(IFilterRegistry filterRegistry)
        {
        }

        public void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
        }

        public void RegisterWithContainer()
        {
            container
                .RegisterType<IMetaWeblogPostService, MetaWeblogPostService>();

            //TODO: (erikpo) Once there is a xml file provider, put this in an if statement based off of the site setting for which provider to use
            container
                .RegisterType<OxiteMetaWeblogDataContext>(new InjectionConstructor(new ResolvedParameter<string>("ApplicationServices")))
                .RegisterType<IMetaWeblogPostRepository, SqlServerMetaWeblogPostRepository>();
        }

        #endregion
    }
}
