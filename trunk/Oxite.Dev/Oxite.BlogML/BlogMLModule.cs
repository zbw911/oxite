//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using System.Web.Routing;
using Oxite.Filters;
using Oxite.Infrastructure;
using Oxite.Modules.BlogML.Controllers;
using Oxite.Modules.Blogs.Filters;
using Oxite.Modules.Blogs.Routing;
using Oxite.Modules.Membership.Filters;
using Microsoft.Practices.Unity;

namespace Oxite.Modules.BlogML
{
    public class BlogMLModule : IOxiteModule
    {
        private readonly IUnityContainer container;

        public BlogMLModule(IUnityContainer container)
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
            string[] controllerNamespaces = new string[] { "Oxite.Modules.BlogML.Controllers" };

            routes.MapRoute(
                "BlogML",
                "Admin/{blogName}/BlogML",
                new { controller = "BlogML", action = "Import", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );
        }

        public void RegisterCatchAllRoutes(RouteCollection routes)
        {
        }

        public void RegisterFilters(IFilterRegistry filterRegistry)
        {
            ControllerActionFilterCriteria listActionsCriteria = new ControllerActionFilterCriteria();
            listActionsCriteria.AddMethod<BlogMLController>(bml => bml.Import(null));
            listActionsCriteria.AddMethod<BlogMLController>(bml => bml.ImportSave(null, null, null));
            filterRegistry.Add(new[] { listActionsCriteria }, typeof(ArchiveListActionFilter));
            filterRegistry.Add(new[] { listActionsCriteria }, typeof(PageSizeActionFilter));

            ControllerActionFilterCriteria adminActionsCriteria = new ControllerActionFilterCriteria();
            adminActionsCriteria.AddMethod<BlogMLController>(bml => bml.Import(null));
            adminActionsCriteria.AddMethod<BlogMLController>(bml => bml.ImportSave(null, null, null));
            filterRegistry.Add(new[] { adminActionsCriteria }, typeof(AuthorizationFilter));
        }

        public void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
        }

        public void RegisterWithContainer()
        {
        }

        #endregion
    }
}
