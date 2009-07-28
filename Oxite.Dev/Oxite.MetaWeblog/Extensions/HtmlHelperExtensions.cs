//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Extensions;
using Oxite.Models;
using System.Web.Mvc;

namespace Oxite.Modules.MetaWeblog.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static void RenderRsd(this HtmlHelper htmlHelper)
        {
            htmlHelper.RenderRsd((Blog)null);
        }

        public static void RenderRsd(this HtmlHelper htmlHelper, Blog blog)
        {
            if (blog != null)
            {
                htmlHelper.RenderRsd(blog.Name);
            }
            else
            {
                htmlHelper.RenderRsd((string)null);
            }
        }

        public static void RenderRsd(this HtmlHelper htmlHelper, string blogName)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            htmlHelper.ViewContext.HttpContext.Response.Write(
                htmlHelper.HeadLink(
                    "EditURI",
                    urlHelper.AbsolutePath(urlHelper.Rsd(blogName)),
                    "application/rsd+xml",
                    "RSD"
                    )
                );
        }

        public static void RenderLiveWriterManifest(this HtmlHelper htmlHelper)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            htmlHelper.RenderLiveWriterManifest(urlHelper.LiveWriterManifest());
        }

        public static void RenderLiveWriterManifest(this HtmlHelper htmlHelper, string path)
        {
            htmlHelper.ViewContext.HttpContext.Response.Write(
                htmlHelper.HeadLink(
                    "wlwmanifest",
                    path,
                    "application/wlwmanifest+xml",
                    ""
                    )
                );
        }
    }
}
