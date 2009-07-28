﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Filters
{
    public class ArchiveListActionFilter : IActionFilter
    {
        private readonly IPostService postService;
        private readonly IBlogService blogService;

        public ArchiveListActionFilter(IPostService postService, IBlogService blogService)
        {
            this.postService = postService;
            this.blogService = blogService;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            OxiteViewModel model = filterContext.Controller.ViewData.GetOxiteViewModel();

            if (model != null)
            {
                IEnumerable<KeyValuePair<ArchiveData, int>> archives;
                INamedEntity container;
                string blogName = filterContext.RouteData.Values["blogName"] as string;

                if (!string.IsNullOrEmpty(blogName))
                {
                    archives = postService.GetArchives(new BlogAddress(blogName));
                    container = blogService.GetBlog(new BlogAddress(blogName));
                }
                else
                {
                    archives = postService.GetArchives();
                    container = new HomePageContainer();
                }

                model.AddModelItem(new ArchiveViewModel(archives, container));
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion
    }
}
