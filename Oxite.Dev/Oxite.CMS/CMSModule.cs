﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Oxite.Infrastructure;
using Oxite.Modules.CMS.Controllers;
using Oxite.Modules.CMS.Filters;
using Oxite.Modules.CMS.Models;
using Oxite.Modules.CMS.ModelBinders;
using Oxite.Modules.CMS.Repositories;
using Oxite.Modules.CMS.Repositories.SqlServer;
using Oxite.Modules.CMS.Routing;
using Oxite.Modules.CMS.Services;
using Oxite.Modules.CMS.Validation;
using Oxite.Modules.Membership.Filters;
using Oxite.Routing;
using Oxite.Validation;

namespace Oxite.Modules.CMS
{
    public class CMSModule : IOxiteModule
    {
        private readonly string[] controllerNamespaces;
        private readonly IUnityContainer container;

        public CMSModule(IUnityContainer container)
        {
            controllerNamespaces = new string[] { "Oxite.Modules.CMS.Controllers" };
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
            routes.MapRoute(
                "Pages",
                "Admin/Pages",
                new { controller = "Page", action = "List", role = "Admin", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "PageAddToSite",
                "Admin/Pages/Add",
                null,
                new { routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "ValidatePageInput",
                "Validation/PageInput",
                new { controller = "Page", action = "Validate" },
                new { httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageAdd",
                "Add",
                new { controller = "Page", action = "Index", pagePath = string.Empty },
                null,
                controllerNamespaces
                );
        }

        public void RegisterCatchAllRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "PageAddToPage",
                "Admin/{*pagePath}",
                new { controller = "Page", action = "ItemAdd", role = "Admin", validateAntiForgeryToken = true },
                new { pagePath = new IsPageMode(PageMode.Add) },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageEdit",
                "Admin/{*pagePath}",
                new { controller = "Page", action = "ItemEdit", role = "Admin", validateAntiForgeryToken = true },
                new { pagePath = new IsPageMode(PageMode.Edit) },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageRemove",
                "Admin/{*pagePath}",
                new { controller = "Page", action = "Remove", role = "Admin", validateAntiForgeryToken = true },
                new { pagePath = new IsPageMode(PageMode.Remove), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "Page",
                "{*pagePath}",
                new { controller = "Page", action = "Item" },
                new { pagePath = new IsPagePath() },
                controllerNamespaces
                );
        }

        public void RegisterFilters(IFilterRegistry filterRegistry)
        {
            ControllerActionFilterCriteria pageListActionCriteria = new ControllerActionFilterCriteria();
            pageListActionCriteria.AddMethod<PageController>(p => p.Add(null, null, null));
            pageListActionCriteria.AddMethod<PageController>(p => p.AddSave(null, null, null));
            pageListActionCriteria.AddMethod<PageController>(p => p.Edit(null, null, null));
            pageListActionCriteria.AddMethod<PageController>(p => p.SaveEdit(null, null, null));
            filterRegistry.Add(new[] { pageListActionCriteria }, typeof(PageListActionFilter));

            ControllerActionFilterCriteria adminActionsCriteria = new ControllerActionFilterCriteria();
            adminActionsCriteria.AddMethod<PageController>(p => p.Add(null, null, null));
            adminActionsCriteria.AddMethod<PageController>(p => p.AddSave(null, null, null));
            adminActionsCriteria.AddMethod<PageController>(p => p.Edit(null, null, null));
            adminActionsCriteria.AddMethod<PageController>(p => p.SaveEdit(null, null, null));
            adminActionsCriteria.AddMethod<PageController>(p => p.Remove(null, null, null));
            filterRegistry.Add(new[] { adminActionsCriteria }, typeof(AuthorizationFilter));
        }

        public void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            modelBinders[typeof(PageAddress)] = new PageAddressModelBinder();
            modelBinders[typeof(PageInput)] = new PageInputModelBinder();
        }

        public void RegisterWithContainer()
        {
            container
                .RegisterType<IPageService, PageService>()
                .RegisterType<IValidator<PageInput>, PageInputValidator>();

            //TODO: (erikpo) Add a check here for if the app is running on sql or xml files
            container
                .RegisterType<OxiteCMSDataContext>(new InjectionConstructor(new ResolvedParameter<string>("ApplicationServices")))
                .RegisterType<IPageRepository, SqlServerPageRepository>();
        }

        #endregion
    }
}
