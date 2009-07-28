//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.Services;
using Oxite.Validation;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Controllers
{
    public class CommentController : Controller
    {
        private readonly IPostService postService;
        private readonly ICommentService commentService;
        private readonly ITagService tagService;
        private readonly IBlogService blogService;

        public CommentController(IPostService postService, ICommentService commentService, ITagService tagService, IBlogService blogService)
        {
            this.postService = postService;
            this.commentService = commentService;
            this.tagService = tagService;
            this.blogService = blogService;
        }

        public OxiteViewModelItems<Comment> List(int? pageNumber, int pageSize, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;

            return new OxiteViewModelItems<Comment>
            {
                Container = new HomePageContainer(),
                Items = commentService.GetComments(pageIndex, pageSize, false, false, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Comment> ListByTag(int? pageNumber, int pageSize, TagAddress tagAddress, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;
            Tag tag = tagService.GetTag(tagAddress);

            if (tag == null) return null;

            return new OxiteViewModelItems<Comment>
            {
                Container = tag,
                Items = commentService.GetComments(pageIndex, pageSize, tag, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Comment> ListByBlog(int? pageNumber, int pageSize, BlogAddress blogAddress, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;
            Blog blog = blogService.GetBlog(blogAddress);

            if (blog == null) return null;

            return new OxiteViewModelItems<Comment>
            {
                Container = blog,
                Items = commentService.GetComments(pageIndex, pageSize, blog, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Comment> ListByPost(int? pageNumber, int pageSize, PostAddress postAddress, UserAuthenticated currentUser)
        {
            Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

            if (post == null) return null;

            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;
            bool includeUnapproved = currentUser != null && currentUser.IsInRole("Admin");

            return new OxiteViewModelItems<Comment>
            {
                Container = post,
                Items = commentService.GetComments(pageIndex, pageSize, post, includeUnapproved, new ServiceContext(ControllerContext, currentUser))
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validate(CommentInput commentInput)
        {
            ValidationStateDictionary validationState = commentService.ValidateCommentInput(commentInput);

            if (validationState.IsValid) return Content("");

            return PartialView("ValidationErrors", new OxiteViewModelPartial<ValidationStateDictionary>(new OxiteViewModel(), validationState));
        }

        public OxiteViewModelItems<Comment> ListForAdmin(int? pageNumber, int pageSize, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;

            return new OxiteViewModelItems<Comment>
            {
                Container = new HomePageContainer(),
                Items = commentService.GetComments(pageIndex, pageSize, true, true, new ServiceContext(ControllerContext, currentUser))
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Approve(CommentAddress commentAddress, string returnUri, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            if (commentService.ApproveComment(commentAddress, new ServiceContext(ControllerContext, currentUser)))
            {
                if (!string.IsNullOrEmpty(returnUri)) return new RedirectResult(returnUri);

                return new JsonResult { Data = true };
            }
            else
            {
                return new JsonResult { Data = false };
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Remove(CommentAddress commentAddress, string returnUri, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            if (commentService.RemoveComment(commentAddress, new ServiceContext(ControllerContext, currentUser)))
            {
                if (!string.IsNullOrEmpty(returnUri)) return new RedirectResult(returnUri);

                return new JsonResult { Data = true };
            }
            else
            {
                return new JsonResult { Data = false };
            }
        }

        public ActionResult CommentOnCommentPartial(CommentAddress commentAddress/*Guid? id*/)
        {
            //TODO: (erikpo) Need to fix
            Comment comment = null; //commentService.GetComment(id ?? Guid.Empty);

            if (comment == null) return Content("");

            return PartialView("CommentOnComment", new OxiteViewModelPartial<Comment>(new OxiteViewModel(), comment));
        }
    }
}
