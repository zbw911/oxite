//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Blogs.Extensions;
using Oxite.Modules.Blogs.Services;
using Oxite.Services;
using Oxite.Validation;
using Oxite.ViewModels;
using Oxite.Modules.Blogs.Models;

namespace Oxite.Modules.Blogs.Controllers
{
    public class PostController : Controller
    {
        private readonly Site site;
        private readonly IBlogService blogService;
        private readonly IPostService postService;
        private readonly ICommentService commentService;
        private readonly ITagService tagService;
        private readonly IPluginEngine pluginEngine;

        public PostController(IBlogService blogService, IPostService postService, ICommentService commentService, ITagService tagService, IPluginEngine pluginEngine, ISiteService siteService)
        {
            this.blogService = blogService;
            this.postService = postService;
            this.commentService = commentService;
            this.tagService = tagService;
            this.pluginEngine = pluginEngine;

            this.site = siteService.GetSite();
            ValidateRequest = false;
        }

        public OxiteViewModelItems<Post> List(int? pageNumber, int pageSize, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;

            return new OxiteViewModelItems<Post>
            {
                Container = new HomePageContainer(),
                Items = postService.GetPosts(pageIndex, pageSize, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Post> ListByBlog(int? pageNumber, int pageSize, BlogAddress blogAddress, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;
            Blog blog = blogService.GetBlog(blogAddress);

            if (blog == null) return null;

            return new OxiteViewModelItems<Post>
            {
                Container = blog,
                Items = postService.GetPosts(pageIndex, pageSize, blogAddress, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Post> ListByTag(int? pageNumber, int pageSize, TagAddress tagAddress, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;
            Tag tag = tagService.GetTag(tagAddress);
            IPageOfItems<Post> posts = postService.GetPosts(pageIndex, pageSize, tagAddress, new ServiceContext(ControllerContext, currentUser));

            if (tag == null || posts.TotalItemCount == 0) return null;

            return new OxiteViewModelItems<Post>
            {
                Container = tag,
                Items = posts
            };
        }

        public OxiteViewModelItems<Post> ListByArchive(int pageSize, ArchiveData archiveData, UserAuthenticated currentUser)
        {
            int pageIndex = archiveData.Page - 1;

            return new OxiteViewModelItems<Post>
            {
                Container = new ArchiveContainer(archiveData),
                Items = postService.GetPosts(pageIndex, pageSize, archiveData, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Post> ListWithDrafts(int? pageNumber, int pageSize, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;

            return new OxiteViewModelItems<Post>
            {
                Container = new HomePageContainer(),
                Items = postService.GetPostsWithDrafts(pageIndex, pageSize, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItems<Post> ListByFileType(int pageSize, string typeName, UserAuthenticated currentUser)
        {
            return new OxiteViewModelItems<Post>()
            {
                Items = postService.GetPostsByFileType(0, pageSize, typeName, new ServiceContext(ControllerContext, currentUser))
            };
        }

        public OxiteViewModelItemItems<Post, Comment> Item(PostAddress postAddress, UserAuthenticated currentUser)
        {
            Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

            if (post == null) return null;

            bool includeUnapproved = currentUser != null && currentUser.IsInRole("Admin");

            return new OxiteViewModelItemItems<Post, Comment>
            {
                Container = post.Blog,
                Item = post,
                Items = commentService.GetComments(0, 25, post, includeUnapproved, new ServiceContext(ControllerContext, currentUser))
            };
        }

        [ActionName("Item"), AcceptVerbs(HttpVerbs.Post)]
        public object AddComment(PostAddress postAddress, CommentInput commentInput, UserAuthenticated currentUser)
        {
            if (site.CommentingDisabled) return null;

            Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

            if (post == null || post.CommentingDisabled) return null;

            ModelResult<Comment> addCommentResults = commentService.AddComment(postAddress, commentInput, new ServiceContext(ControllerContext, currentUser));

            if (!addCommentResults.IsValid)
            {
                ModelState.AddModelErrors(addCommentResults.ValidationState);

                return Item(postAddress, currentUser);
            }

            if (currentUser == null)
            {
                if (commentInput.SaveAnonymousUser)
                    Response.Cookies.SetAnonymousUser(commentInput.Creator);
                else if (Request.Cookies.GetAnonymousUser() != null)
                    Response.Cookies.ClearAnonymousUser();
            }

            return new RedirectResult(
                addCommentResults.Item.State != EntityState.PendingApproval
                ? Url.Comment(addCommentResults.Item)
                : Url.CommentPending(addCommentResults.Item)
                );
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validate(PostInput postInput)
        {
            ValidationStateDictionary validationState = postService.ValidatePostInput(postInput);

            if (validationState.IsValid) return Content("");

            return PartialView("ValidationErrors", new OxiteViewModelPartial<ValidationStateDictionary>(new OxiteViewModel(), validationState));
        }

        [ActionName("ItemAdd"), AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<PostInput> Add(BlogAddress blogAddress, PostInput postInput)
        {
            //TODO: (erikpo) Check permissions

            Blog blog = blogAddress != null ? blogService.GetBlog(blogAddress) : null;

            return new OxiteViewModelItem<PostInput>
            {
                Item = new PostInput(blog, postInput)
            };
        }

        [ActionName("ItemAdd"), AcceptVerbs(HttpVerbs.Post)]
        public object AddSave(BlogAddress blogAddress, PostInput postInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<Post> results = postService.AddPost(postInput, EntityState.Normal, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return Add(blogAddress, postInput);
            }

            return Redirect(Url.Post(results.Item));
        }

        [ActionName("ItemEdit"), AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<PostInput> Edit(PostAddress postAddress, PostInput postInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

            if (post == null) { return null; }

            return new OxiteViewModelItem<PostInput>
            {
                Item = new PostInput(post, postInput)
            };
        }

        [ActionName("ItemEdit"), AcceptVerbs(HttpVerbs.Post)]
        public object EditSave(PostAddress postAddress, PostInput postInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<Post> results = postService.EditPost(postAddress, postInput, EntityState.Normal, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return Edit(postAddress, postInput, currentUser);
            }

            return Redirect(Url.Post(results.Item));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Remove(PostAddress postAddress, string returnUri, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            postService.RemovePost(postAddress, new ServiceContext(ControllerContext, currentUser));

            return Redirect(returnUri);
        }
    }
}
