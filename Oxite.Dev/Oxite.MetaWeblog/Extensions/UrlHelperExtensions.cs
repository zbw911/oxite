//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;

namespace Oxite.Modules.MetaWeblog.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string MetaWeblog(this UrlHelper urlHelper)
        {
            return urlHelper.AppPath("/MetaWeblog");
        }

        public static string Rsd(this UrlHelper urlHelper)
        {
            return urlHelper.Rsd((Blog)null);
        }

        public static string Rsd(this UrlHelper urlHelper, Blog blog)
        {
            if (blog != null)
                return urlHelper.Rsd(blog.Name);

            return urlHelper.Rsd((string)null);
        }

        public static string Rsd(this UrlHelper urlHelper, string blogName)
        {
            if (!string.IsNullOrEmpty(blogName))
                return urlHelper.RouteUrl("BlogRsd", new { blogName });

            return urlHelper.RouteUrl("Rsd");
        }

        public static string LiveWriterManifest(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("LiveWriterManifest");
        }
    }
}
