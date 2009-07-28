//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Filters
{
    public class TagCloudActionFilter : IActionFilter
    {
        private readonly ITagService tagService;
        private readonly IBlogService blogService;

        public TagCloudActionFilter(ITagService tagService, IBlogService blogService)
        {
            this.tagService = tagService;
            this.blogService = blogService;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            OxiteViewModel model = filterContext.Controller.ViewData.GetOxiteViewModel();

            if (model != null)
            {
                string blogName = filterContext.RouteData.Values["blogName"] as string;

                if (blogName != null)
                    model.AddModelItem(new TagCloudViewModel(tagService.GetTagsUsedIn(new BlogAddress(blogName))));
                else
                    model.AddModelItem(new TagCloudViewModel(tagService.GetTagsWithPostCount()));
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion
    }
}
