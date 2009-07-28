//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Services;

namespace Oxite.Filters
{
    public class ViewEnginesResultFilter : IResultFilter, IExceptionFilter
    {
        private readonly IUnityContainer container;
        private readonly ISkinResolverRegistry skinResolvers;
        private readonly Site site;

        public ViewEnginesResultFilter(IUnityContainer container, ISkinResolverRegistry skinResolvers, ISiteService siteService)
        {
            this.container = container;
            this.skinResolvers = skinResolvers;
            site = siteService.GetSite();
        }

        #region IResultFilter Members

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            setupSkinViewEngines(filterContext.RequestContext, filterContext.Controller.ViewData, filterContext.Result as ViewResultBase);
        }

        #endregion

        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            setupSkinViewEngines(filterContext.RequestContext, filterContext.Controller.ViewData, filterContext.Result as ViewResultBase);
        }

        #endregion

        private void setupSkinViewEngines(RequestContext requestContext, ViewDataDictionary viewData, ViewResultBase result)
        {
            HttpRequestBase request = requestContext.HttpContext.Request;
            string appPath = request.ApplicationPath != "/" ? request.ApplicationPath : "";
            string queryStringSkinName = request.QueryString["skin"];
            string cookieSkinName = request.Cookies.GetSkinName();
            string skin = "";

            if (!string.IsNullOrEmpty(queryStringSkinName))
                skin = queryStringSkinName;
            else if (!string.IsNullOrEmpty(cookieSkinName))
                skin = cookieSkinName;

            if (skin == "" && request.Url.PathAndQuery.StartsWith(appPath + "/Admin", StringComparison.OrdinalIgnoreCase))
                skin = site.AdminSkin;

            if (skin == "")
                skin = !string.IsNullOrEmpty(viewData["Skin"] as string)
                    ? viewData["Skin"] as string
                    : site.Skin;

            IEnumerable<IOxiteViewEngine> viewEngines = skinResolvers.GenerateViewEngines(new SkinResolverContext(requestContext, skin), skin);

            if (result != null)
            {
                result.ViewEngineCollection = new ViewEngineCollection(viewEngines.Cast<IViewEngine>().ToList());

                result.ViewData["OxiteViewEngines"] = viewEngines;
            }

            viewData["OxiteViewEngines"] = viewEngines;
        }
    }
}
