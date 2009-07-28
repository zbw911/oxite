//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Filters
{
    public class DashboardDataActionFilter : IActionFilter
    {
        private readonly IBlogService blogService;
        private readonly IPostService postService;
        private readonly ICommentService commentService;

        public DashboardDataActionFilter(IBlogService blogService, IPostService postService, ICommentService commentService)
        {
            this.blogService = blogService;
            this.postService = postService;
            this.commentService = commentService;
        }

        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            OxiteViewModel model = filterContext.Controller.ViewData.GetOxiteViewModel();

            if (model != null)
            {
                ServiceContext serviceContext = new ServiceContext(filterContext.HttpContext, filterContext.RouteData, model.User.ToUserAuthenticated());

                //recent posts - all up
                IEnumerable<Post> posts = postService.GetPostsWithDrafts(0, 5, serviceContext);

                //recent comments - all up
                IEnumerable<Comment> comments = commentService.GetComments(0, 10, true, true, serviceContext);

                model.AddModelItem(new AdminDataViewModel(posts, comments));
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        #endregion
    }
}
