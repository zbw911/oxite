//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Services;
using Oxite.ViewModels;

namespace Oxite.Filters
{
    public class SiteActionFilter : IActionFilter, IExceptionFilter
    {
        private readonly IUnityContainer container;

        public SiteActionFilter(IUnityContainer container)
        {
            this.container = container;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            setModel(filterContext.Controller.ViewData.GetOxiteViewModel());
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            setModel(filterContext.Controller.ViewData.GetOxiteViewModel());
        }

        #endregion

        private void setModel(OxiteViewModel model)
        {
            if (model != null)
                model.Site = new SiteViewModel(container.Resolve<ISiteService>().GetSite(), container.Resolve<OxiteConfigurationSection>().InstanceName);
        }
    }
}
