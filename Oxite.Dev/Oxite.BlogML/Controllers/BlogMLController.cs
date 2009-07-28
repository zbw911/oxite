//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Web.Mvc;
using System.Xml;
using BlogML.Xml;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.BlogML.Extensions;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.Services;
using Oxite.Validation;
using Oxite.ViewModels;

namespace Oxite.Modules.BlogML.Controllers
{
    public class BlogMLController : Controller
    {
        private readonly Site site;
        private readonly ISiteService siteService;
        private readonly ILanguageService languageService;
        private readonly IBlogService blogService;
        private readonly IPostService postService;
        private readonly ICommentService commentService;

        public BlogMLController(ISiteService siteService, ILanguageService languageService, IBlogService blogService, IPostService postService, ICommentService commentService)
        {
            this.siteService = siteService;
            this.site = siteService.GetSite();
            this.languageService = languageService;
            this.blogService = blogService;
            this.postService = postService;
            this.commentService = commentService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public virtual OxiteViewModel Import(BlogAddress blogAddress)
        {
            //TODO: (erikpo) Change this to be user selectable in the multiple blogs case if the user is a site owner
            Blog blog = blogService.GetBlog(blogAddress);

            if (blog == null) return null;

            return new OxiteViewModel { Container = blog };
        }

        [ActionName("Import"), AcceptVerbs(HttpVerbs.Post)]
        public virtual OxiteViewModel ImportSave(BlogAddress blogAddress, string slugPattern, UserAuthenticated currentUser)
        {
            ServiceContext serviceContext = new ServiceContext(ControllerContext, currentUser);
            Blog blog = blogService.GetBlog(blogAddress);

            if (blog == null) return null;

            ValidationStateDictionary validationState = new ValidationStateDictionary();
            XmlTextReader reader = null;
            bool modifiedSite = false;

            try
            {
                reader = new XmlTextReader(Request.Files[0].InputStream);

                BlogMLBlog blogMLBlog = BlogMLSerializer.Deserialize(reader);
                Language language = languageService.GetLanguage(site.LanguageDefault);
                ImportBlogInput blogInput = new ImportBlogInput(blogMLBlog.SubTitle, blogMLBlog.SubTitle, blogMLBlog.DateCreated);
                ModelResult<Blog> results = blogService.EditBlog(blogAddress, blogInput, serviceContext);

                if (!results.IsValid)
                {
                    ModelState.AddModelErrors(results.ValidationState);

                    return Import(blogAddress);
                }

                if (!site.HasMultipleBlogs)
                {
                    site.DisplayName = blog.DisplayName;
                    site.Description = blog.Description;

                    siteService.EditSite(site, out validationState);

                    if (!validationState.IsValid) throw new Exception();

                    modifiedSite = true;
                }

                postService.RemoveAll(blogAddress);

                foreach (BlogMLPost blogMLPost in blogMLBlog.Posts)
                {
                    if (string.IsNullOrEmpty(blogMLPost.Title) || string.IsNullOrEmpty(blogMLPost.Content.Text))
                        continue;

                    ImportPostInput postInput = blogMLPost.ToImportPostInput(blogMLBlog, site.CommentingDisabled | blog.CommentingDisabled, slugPattern, blogMLPost.Approved ? EntityState.Normal : EntityState.PendingApproval, currentUser);
                    ModelResult<Post> addPostResults = postService.AddPost(blog, postInput, serviceContext);

                    if (!addPostResults.IsValid)
                    {
                        ModelState.AddModelErrors(addPostResults.ValidationState);

                        return Import(blogAddress);
                    }

                    foreach (BlogMLComment blogMLComment in blogMLPost.Comments)
                    {
                        ImportCommentInput commentInput = blogMLComment.ToImportCommentInput(blogMLBlog, currentUser, language);
                        ModelResult<Comment> addCommentResults = commentService.AddComment(addPostResults.Item, commentInput, serviceContext);

                        if (!addCommentResults.IsValid)
                        {
                            ModelState.AddModelErrors(addCommentResults.ValidationState);

                            return Import(blogAddress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelErrors(validationState);

                if (!string.IsNullOrEmpty(ex.Message))
                    ModelState.AddModelError("ModelName", ex);

                return Import(blogAddress);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            if (modifiedSite)
                OxiteApplication.Load(ControllerContext.HttpContext);

            return new OxiteViewModel { Container = blog };
        }

        //TODO: (erikpo) Need to add Export actions
    }
}
