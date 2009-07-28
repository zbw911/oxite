// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Extensions
{
    public static class HtmlHelperExtensions
    {
        #region PingbackDiscovery

        public static string PingbackDiscovery(this HtmlHelper htmlHelper, Post post)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            return htmlHelper.HeadLink("pingback", urlHelper.AbsolutePath(urlHelper.Pingback(post)), "", "");
        }

        #endregion

        #region TrackbackBlock

        public static string TrackbackBlock<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : OxiteViewModel
        {
            OxiteViewModelItem<Post> model = htmlHelper.ViewData.Model as OxiteViewModelItem<Post>;

            if (model != null)
            {
                Post post = model.Item;

                if (post != null)
                {
                    UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

                    return string.Format("<!--<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:trackback=\"http://madskills.com/public/xml/rss/module/trackback/\"><rdf:Description rdf:about=\"{2}\" dc:identifier=\"{2}\" dc:title=\"{0}\" trackback:ping=\"{1}\" /></rdf:RDF>-->", htmlHelper.Encode(post.Title), urlHelper.AbsolutePath(urlHelper.Trackback(post)), urlHelper.AbsolutePath(urlHelper.Post(post)));
                }
            }

            return "";
        }

        #endregion
    }
}
