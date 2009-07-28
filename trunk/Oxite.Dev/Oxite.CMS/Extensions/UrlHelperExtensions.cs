//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Models;
using Oxite.Modules.CMS.Models;

namespace Oxite.Modules.CMS.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Pages(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Pages");
        }

        public static string Page(this UrlHelper urlHelper, Page page)
        {
            return urlHelper.Page(page.Path.Substring(1));
        }

        public static string Page(this UrlHelper urlHelper, string pagePath)
        {
            return urlHelper.RouteUrl("Page", new { pagePath });
        }

        public static string ValidatePageInput(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("ValidatePageInput");
        }

        public static string PageAdd(this UrlHelper urlHelper, Page page)
        {
            return urlHelper.PageAdd(page.Path.Substring(1));
        }

        public static string PageAdd(this UrlHelper urlHelper)
        {
            return urlHelper.PageAdd(urlHelper.RequestContext.RouteData.Values["pagePath"] as string);
        }

        public static string PageAdd(this UrlHelper urlHelper, string pagePath)
        {
            if (string.IsNullOrEmpty(pagePath))
                pagePath = urlHelper.RequestContext.RouteData.Values["pagePath"] as string;

            if (!string.IsNullOrEmpty(pagePath))
            {
                if (pagePath.EndsWith("/" + PageMode.Add))
                    pagePath = pagePath.Substring(0, pagePath.Length - ("/" + PageMode.Add).Length);
                else if (pagePath.EndsWith("/" + PageMode.Edit))
                    pagePath = pagePath.Substring(0, pagePath.Length - ("/" + PageMode.Edit).Length);
                else if (pagePath.EndsWith("/" + PageMode.Remove))
                    pagePath = pagePath.Substring(0, pagePath.Length - ("/" + PageMode.Remove).Length);
            }

            return !string.IsNullOrEmpty(pagePath)
                ? urlHelper.RouteUrl("PageAddToPage", new { pagePath = pagePath + "/Add" })
                : urlHelper.RouteUrl("PageAddToSite");
        }

        public static string PageEdit(this UrlHelper urlHelper, Page page)
        {
            return urlHelper.PageEdit(page.Path.Substring(1));
        }

        public static string PageEdit(this UrlHelper urlHelper, string pagePath)
        {
            return urlHelper.RouteUrl("PageEdit", new { pagePath = pagePath + "/Edit" });
        }

        public static string PageRemove(this UrlHelper urlHelper, Page page)
        {
            return urlHelper.PageRemove(page.Path.Substring(1));
        }

        public static string PageRemove(this UrlHelper urlHelper, string pagePath)
        {
            return urlHelper.RouteUrl("PageRemove", new { pagePath = pagePath + "/Remove" });
        }
    }
}
