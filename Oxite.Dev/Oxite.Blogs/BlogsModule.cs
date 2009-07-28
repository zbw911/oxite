//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Oxite.Filters;
using Oxite.Infrastructure;
using Oxite.Infrastructure.XmlRpc;
using Oxite.Modules.Blogs.Controllers;
using Oxite.Modules.Blogs.Filters;
using Oxite.Modules.Blogs.ModelBinders;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Repositories;
using Oxite.Modules.Blogs.Repositories.SqlServer;
using Oxite.Modules.Blogs.Routing;
using Oxite.Modules.Blogs.Services;
using Oxite.Modules.Blogs.Validation;
using Oxite.Modules.Core.Controllers;
using Oxite.Modules.Membership.Filters;
using Oxite.Routing;
using Oxite.Validation;

namespace Oxite.Modules.Blogs
{
    public class BlogsModule : IOxiteModule
    {
        private readonly IUnityContainer container;

        public BlogsModule(IUnityContainer container)
        {
            this.container = container;
        }

        #region IOxiteModule Members

        public void Initialize()
        {
        }

        public void Unload()
        {
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            string[] controllerNamespaces = new string[] { "Oxite.Modules.Blogs.Controllers" };

            // Site Map

            routes.MapRoute(
                "SiteMapIndex",
                "SiteMap",
                new { controller = "SiteMap", action = "SiteMapIndex" },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "SiteMap",
                "SiteMap/{year}/{month}",
                new { controller = "SiteMap", action = "SiteMap" },
                new
                {
                    year = new IsInt(DateTime.MinValue.Year, DateTime.MaxValue.Year),
                    month = new IsInt(DateTime.MinValue.Month, DateTime.MaxValue.Month)
                },
                controllerNamespaces
                );

            // Trackbacks

            routes.MapRoute(
                "Trackback",
                "{blogName}/{postSlug}/Trackback",
                new { controller = "Trackback", action = "Add" },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            // Files

            routes.MapRoute(
                "FilesByPost",
                "Admin/{blogName}/{postSlug}/Files",
                new { controller = "File", action = "ListByPost", role = "Admin" },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "AddFileContentToPost",
                "Admin/{blogName}/{postSlug}/AddFileContent",
                new { controller = "File", action = "AddFileContentToPost", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "AddFileToPost",
                "Admin/{blogName}/{postSlug}/AddFile",
                new { controller = "File", action = "AddFileToPost", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "EditFileOnPost",
                "Admin/{blogName}/{postSlug}/EditFileOnPost",
                new { controller = "File", action = "EditFileOnPost", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "RemoveFileFromPost",
                "Admin/{blogName}/{postSlug}/RemoveFile",
                new { controller = "File", action = "RemoveFileFromPost", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            // Tags

            routes.MapRoute(
                "Tags",
                "Tags",
                new { controller = "Tag", action = "Cloud" },
                null,
                controllerNamespaces
                );

            // Comments

            routes.MapRoute(
                "ValidateCommentInput",
                "Validation/CommentInput",
                new { controller = "Comment", action = "Validate" },
                new { httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "RemoveComment",
                "Admin/{blogName}/{postSlug}/{commentSlug}/RemoveComment",
                new { controller = "Comment", action = "Remove", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "ApproveComment",
                "Admin/{blogName}/{postSlug}/{commentSlug}/ApproveComment",
                new { controller = "Comment", action = "Approve", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "AllComments",
                "Admin/Comments",
                new { controller = "Comment", action = "ListForAdmin", role = "Admin" },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "PageOfAllComments",
                "Admin/Comments/page{pageNumber}",
                new { controller = "Comment", action = "ListForAdmin", role = "Admin" },
                new { pageNumber = new IsInt() },
                controllerNamespaces
                );

            routes.MapRoute(
                "AllCommentsPermalink",
                "Admin/Comments#{comment}",
                null,
                new { routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "Comments",
                "Comments/{dataFormat}",
                new { controller = "Comment", action = "List" },
                new { dataFormat = "(RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "CommentsByBlog",
                "{blogName}/Comments/{dataFormat}",
                new { controller = "Comment", action = "ListByBlog" },
                new { blogName = new BlogConstraint(container), dataFormat = "(RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "CommentsByPost",
                "{blogName}/{postSlug}/Comments/{dataFormat}",
                new { controller = "Comment", action = "ListByPost" },
                new { blogName = new BlogConstraint(container), dataformat = "(RSS|ATOM|JSON)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "CommentOnCommentPartial",
                "{blogName}/{postSlug}/{commentSlug}/CommentOnCommentPartial",
                new { controller = "Comment", action = "CommentOnCommentPartial" },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostCommentPermalink",
                "{blogName}/{postSlug}#{commentSlug}",
                null,
                new { blogName = new BlogConstraint(container), routeDirection = new RouteDirectionConstraint(RouteDirection.UrlGeneration) },
                controllerNamespaces
                );

            routes.MapRoute(
                "CommentsByTag",
                "Tags/{tagName}/Comments/{dataFormat}",
                new { controller = "Comment", action = "ListByTag" },
                new { dataFormat = "(RSS|ATOM)" },
                controllerNamespaces
                );

            // Posts

            routes.MapRoute(
                "PostAddToSite",
                "Admin/AddPost",
                new { controller = "Post", action = "ItemAdd", role = "Admin", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "PostAddToBlog",
                "Admin/{blogName}/AddPost",
                new { controller = "Post", action = "ItemAdd", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostEdit",
                "Admin/{blogName}/{postSlug}/Edit",
                new { controller = "Post", action = "ItemEdit", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostRemove",
                "Admin/{blogName}/{postSlug}/Remove",
                new { controller = "Post", action = "Remove", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostsWithDrafts",
                "Admin/Posts",
                new { controller = "Post", action = "ListWithDrafts", role = "Admin" },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "PageOfPostsWithDrafts",
                "Admin/Posts/page{pageNumber}",
                new { controller = "Post", action = "ListWithDrafts", role = "Admin" },
                new { pageNumber = new IsInt() },
                controllerNamespaces
                );

            routes.MapRoute(
                "ValidatePostInput",
                "Validation/PostInput",
                new { controller = "Post", action = "Validate" },
                new { httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "Posts",
                "{dataFormat}",
                new { controller = "Post", action = "List", dataFormat = "" },
                new { dataFormat = "(|RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageOfPosts",
                "page{pageNumber}",
                new { controller = "Post", action = "List" },
                new { pageNumber = new IsInt() },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostsByBlog",
                "{blogName}/{dataFormat}",
                new { controller = "Post", action = "ListByBlog", dataFormat = "" },
                new { blogName = new BlogConstraint(container), dataFormat = "(|RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageOfPostsByBlog",
                "{blogName}/page{pageNumber}",
                new { controller = "Post", action = "ListByBlog" },
                new { blogName = new BlogConstraint(container), pageNumber = new IsInt() },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostsByBlogArchive",
                "{blogName}/Archive/{*archiveData}",
                new { controller = "Post", action = "ListByArchive", archiveData = Oxite.Models.ArchiveData.DefaultString },
                new { blogName = new BlogConstraint(container), archiveData = new IsArchiveData() },
                controllerNamespaces
                );

            routes.MapRoute(
                "AddCommentToPostAsUser",
                "{blogName}/{postSlug}",
                new { controller = "Post", action = "Item", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST"), authenticated = new IsAuthenticated() },
                controllerNamespaces
                );

            routes.MapRoute(
                "AddCommentToPost",
                "{blogName}/{postSlug}",
                new { controller = "Post", action = "Item" },
                new { blogName = new BlogConstraint(container), httpMethod = new HttpMethodConstraint("POST") },
                controllerNamespaces
                );

            routes.MapRoute(
                "Post",
                "{blogName}/{postSlug}/{dataFormat}",
                new { controller = "Post", action = "Item", dataFormat = "" },
                new { blogName = new BlogConstraint(container), dataformat = "(|RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostsByTag",
                "Tags/{tagName}/{dataFormat}",
                new { controller = "Post", action = "ListByTag", dataFormat = "" },
                new { dataFormat = "(|RSS|ATOM)" },
                controllerNamespaces
                );

            routes.MapRoute(
                "PageOfPostsByTag",
                "Tags/{tagName}/page{pageNumber}",
                new { controller = "Post", action = "ListByTag" },
                new { pageNumber = new IsInt() },
                controllerNamespaces
                );

            routes.MapRoute(
                "PostsByArchive",
                "Archive/{*archiveData}",
                new { controller = "Post", action = "ListByArchive" },
                new { archiveData = new IsArchiveData() },
                controllerNamespaces
                );

            // Blogs

            routes.MapRoute(
                "BlogFind",
                "Admin/Blogs",
                new { controller = "Blog", action = "Find", role = "Admin", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "BlogAdd",
                "Admin/Blogs/Add",
                new { controller = "Blog", action = "ItemEdit", role = "Admin", validateAntiForgeryToken = true },
                null,
                controllerNamespaces
                );

            routes.MapRoute(
                "BlogEdit",
                "Admin/Blogs/{blogName}/Edit",
                new { controller = "Blog", action = "ItemEdit", role = "Admin", validateAntiForgeryToken = true },
                new { blogName = new BlogConstraint(container) },
                controllerNamespaces
                );

            routes.Add(
                "Pingback",
                new Route(
                    "Pingback",
                    new RouteValueDictionary(new Dictionary<string, object> { { "controller", "Pingback" } }),
                    new XmlRpcRouteHandler()
                    )
                );
        }

        public void RegisterCatchAllRoutes(RouteCollection routes)
        {
        }

        public void RegisterFilters(IFilterRegistry filterRegistry)
        {
            ControllerActionFilterCriteria listActionsCriteria = new ControllerActionFilterCriteria();
            listActionsCriteria.AddMethod<BlogController>(a => a.Find());
            listActionsCriteria.AddMethod<BlogController>(a => a.FindQuery(null));
            listActionsCriteria.AddMethod<PostController>(p => p.List(null, 0, null));
            listActionsCriteria.AddMethod<PostController>(p => p.ListByArchive(0, null, null));
            listActionsCriteria.AddMethod<PostController>(p => p.ListByBlog(null, 0, null, null));
            listActionsCriteria.AddMethod<PostController>(p => p.ListByTag(null, 0, null, null));
            listActionsCriteria.AddMethod<PostController>(p => p.ListWithDrafts(null, 0, null));
            listActionsCriteria.AddMethod<CommentController>(c => c.List(0, 0, null));
            listActionsCriteria.AddMethod<CommentController>(c => c.ListByPost(null, 0, null, null));
            listActionsCriteria.AddMethod<CommentController>(c => c.ListByTag(null, 0, null, null));
            listActionsCriteria.AddMethod<CommentController>(c => c.ListByBlog(null, 0, null, null));
            filterRegistry.Add(new[] { listActionsCriteria }, typeof(ArchiveListActionFilter));
            filterRegistry.Add(new[] { listActionsCriteria }, typeof(PageSizeActionFilter));

            ControllerActionFilterCriteria tagCloudActionCriteria = new ControllerActionFilterCriteria();
            tagCloudActionCriteria.AddMethod<TagController>(t => t.Cloud());
            filterRegistry.Add(new[] { tagCloudActionCriteria }, typeof(ArchiveListActionFilter));

            ControllerActionFilterCriteria itemActionCriteria = new ControllerActionFilterCriteria();
            itemActionCriteria.AddMethod<PostController>(p => p.Item(null, null));
            itemActionCriteria.AddMethod<PostController>(p => p.AddComment(null, null, null));
            filterRegistry.Add(new[] { itemActionCriteria }, typeof(CommentingDisabledActionFilter));

            ControllerActionFilterCriteria userBlogListActionCriteria = new ControllerActionFilterCriteria();
            userBlogListActionCriteria.AddMethod<PostController>(p => p.Add(null, null));
            userBlogListActionCriteria.AddMethod<PostController>(p => p.AddSave(null, null, null));
            userBlogListActionCriteria.AddMethod<PostController>(p => p.Edit(null, null, null));
            userBlogListActionCriteria.AddMethod<PostController>(p => p.EditSave(null, null, null));
            filterRegistry.Add(new[] { userBlogListActionCriteria }, typeof(UserBlogListActionFilter));

            ControllerActionFilterCriteria adminActionsCriteria = new ControllerActionFilterCriteria();
            adminActionsCriteria.AddMethod<BlogController>(a => a.Find());
            adminActionsCriteria.AddMethod<BlogController>(a => a.FindQuery(null));
            adminActionsCriteria.AddMethod<BlogController>(a => a.ItemEdit(null));
            adminActionsCriteria.AddMethod<BlogController>(a => a.ItemSave(null, null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.Add(null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.AddSave(null, null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.Edit(null, null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.EditSave(null, null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.Remove(null, null, null));
            adminActionsCriteria.AddMethod<PostController>(p => p.ListWithDrafts(null, 0, null));
            //adminActionsCriteria.AddMethod<CommentController>(c => c.List(0, 0, null));
            adminActionsCriteria.AddMethod<CommentController>(c => c.Remove(null, null, null));
            adminActionsCriteria.AddMethod<CommentController>(c => c.Approve(null, null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.ListByPost(null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.AddFileContentToPost(null, null, null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.AddFileToPost(null, null, null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.EditFileContentOnPost(null, null, null, null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.EditFileOnPost(null, null, null, null, null));
            adminActionsCriteria.AddMethod<FileController>(f => f.RemoveFileFromPost(null, null, null));
            filterRegistry.Add(new[] { adminActionsCriteria }, typeof(AuthorizationFilter));

            ControllerActionFilterCriteria dashboardDataActionCriteria = new ControllerActionFilterCriteria();
            dashboardDataActionCriteria.AddMethod<SiteController>(s => s.Dashboard());
            filterRegistry.Add(new[] { dashboardDataActionCriteria }, typeof(DashboardDataActionFilter));

            filterRegistry.Add(new[] { new XmlRpcFilterCriteria() }, typeof(XmlRpcFaultExceptionFilter));
        }

        public void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            modelBinders[typeof(BlogAddress)] = new BlogAddressModelBinder();
            modelBinders[typeof(BlogInput)] = new BlogInputModelBinder();
            modelBinders[typeof(BlogSearchCriteria)] = new BlogSearchCriteriaModelBinder();
            modelBinders[typeof(PostAddress)] = new PostAddressModelBinder();
            modelBinders[typeof(PostInput)] = new PostInputModelBinder();
            modelBinders[typeof(CommentAddress)] = new CommentAddressModelBinder();
            modelBinders[typeof(CommentInput)] = new CommentInputModelBinder();
            modelBinders[typeof(TagAddress)] = new TagAddressModelBinder();
            modelBinders[typeof(FileAddress)] = new FileAddressModelBinder();
            modelBinders[typeof(FileInput)] = new FileInputModelBinder();
            modelBinders[typeof(FileContentInput)] = new FileContentInputModelBinder();
        }

        public void RegisterWithContainer()
        {
            container
                .RegisterType<IBlogService, BlogService>()
                .RegisterType<IValidator<BlogInput>, BlogInputValidator>()
                .RegisterType<IPostService, PostService>()
                .RegisterType<IValidator<PostInput>, PostInputValidator>()
                .RegisterType<ICommentService, CommentService>()
                .RegisterType<IValidator<CommentInput>, CommentInputValidator>()
                .RegisterType<ITagService, TagService>()
                .RegisterType<ITrackbackOutboundService, TrackbackOutboundService>();

            //TODO: (erikpo) Once there is a xml file provider, put this in an if statement based off of the site setting for which provider to use
            container
                .RegisterType<OxiteBlogsDataContext>(new InjectionConstructor(new ResolvedParameter<string>("ApplicationServices")))
                .RegisterType<IBlogRepository, SqlServerBlogRepository>()
                .RegisterType<IPostRepository, SqlServerPostRepository>()
                .RegisterType<ICommentRepository, SqlServerCommentRepository>()
                .RegisterType<ITagRepository, SqlServerTagRepository>()
                .RegisterType<ITrackbackOutboundRepository, SqlServerTrackbackOutboundRepository>();
        }

        #endregion
    }
}
