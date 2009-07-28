//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Infrastructure.XmlRpc;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.Modules.FormsAuthentication.Extensions;
using Oxite.Modules.Membership.Services;
using Oxite.Modules.MetaWeblog.Extensions;
using Oxite.Modules.MetaWeblog.Services;
using Oxite.Services;

namespace Oxite.Modules.MetaWeblog.Controllers
{
    public class MetaWeblogController : Controller
    {
        private readonly Site site;
        private readonly IBlogService blogService;
        private readonly IPostService postService;
        private readonly IMetaWeblogPostService metaWeblogPostService;
        private readonly ITagService tagService;
        private readonly IUserService userService;
        private readonly IRegularExpressions expressions;

        public MetaWeblogController(ISiteService siteService, IBlogService blogService, IPostService postService, IMetaWeblogPostService metaWeblogPostService, ITagService tagService, IUserService userService, IRegularExpressions expressions)
        {
            this.site = siteService.GetSite();
            this.blogService = blogService;
            this.postService = postService;
            this.metaWeblogPostService = metaWeblogPostService;
            this.tagService = tagService;
            this.userService = userService;
            this.expressions = expressions;
        }

        public ContentResult Rsd(BlogAddress blogAddress)
        {
            return Content(GenerateRsd(blogAddress).ToString(), "text/xml");
        }

        protected XDocument GenerateRsd(BlogAddress blogAddress)
        {
            XNamespace rsdNamespace = "http://archipelago.phrasewise.com/rsd";
            XDocument rsd = new XDocument(
                new XElement(rsdNamespace + "rsd", new XAttribute("version", "1.0"),
                             new XElement(rsdNamespace + "service",
                                          new XElement(rsdNamespace + "engineName", "Oxite"),
                                          new XElement(rsdNamespace + "engineLink", Url.Oxite()),
                                          new XElement(rsdNamespace + "homePageLink", Url.AbsolutePath(Url.Home())),
                                          new XElement(rsdNamespace + "apis",
                                                       GenerateRsdApiList(blogAddress, rsdNamespace)
                                 )
                    )
                ));

            return rsd;
        }

        protected XElement[] GenerateRsdApiList(BlogAddress blogAddress, XNamespace rsdNamespace)
        {
            //TODO: (erikpo) Make this return only the current blog
            IEnumerable<Blog> blogs = null; //blogService.GetBlogs();
            List<XElement> elements = new List<XElement>(blogs.Count());

            string apiLink = new UriBuilder(site.Host) { Path = Url.MetaWeblog() }.Uri.ToString();

            foreach (Blog blog in blogs)
            {
                elements.Add(
                    new XElement(
                        rsdNamespace + "api",
                        new XAttribute("name", "MetaWeblog"),
                        new XAttribute("blogID", blog.ID.ToString("N")),
                        new XAttribute(
                            "preferred",
                            (blogs.Count() == 1 || string.Compare(blog.Name, blogAddress.BlogName, true) == 0).ToString().ToLower()
                            ),
                        new XAttribute("apiLink", apiLink)
                        )
                    );
            }

            return elements.ToArray();
        }

        [ActionName("metaWeblog.newPost")]
        public ActionResult NewPost(string blogId, string username, string password, IDictionary<string, object> post, bool publish)
        {
            if (string.IsNullOrEmpty(blogId)) throw new ArgumentException();

            UserAuthenticated user = Get_user(username, password);
            Blog blog = blogService.GetBlog(new BlogAddress(blogId));

            //TODO: (erikpo) Move into a model binder?
            PostInput postInput = new PostInput(blog.Name, post["title"] as string, post["description"] as string,
                                                post["mt_excerpt"] as string,
                                                (post["categories"] as object[]).OfType<string>() ??
                                                Enumerable.Empty<string>(),
                                                string.IsNullOrEmpty(post["mt_basename"] as string)
                                                    ? expressions.Slugify(post["title"] as string)
                                                    : post["mt_basename"] as string,
                                                publish ? DateTime.UtcNow : (DateTime?) null,
                                                blog.CommentingDisabled);

            ModelResult<Post> results = postService.AddPost(postInput, EntityState.Normal, new ServiceContext(HttpContext, RouteData, user));

            if (results.IsValid)
                return new XmlRpcResult(results.Item.ID.ToString());

            return new XmlRpcFaultResult(0, results.GetFirstException().Message);
        }

        [ActionName("metaWeblog.editPost")]
        public ActionResult EditPost(string postId, string username, string password, IDictionary<string, object> post, bool publish)
        {
            if (string.IsNullOrEmpty(postId)) throw new ArgumentException();

            UserAuthenticated user = Get_user(username, password);
            Post existingPost = metaWeblogPostService.GetPost(new MetaWeblogPostAddress(new Guid(postId)), new ServiceContext(HttpContext, new RouteData(), user));

            //TODO: (erikpo) Move into a model binder?
            PostInput postInput = new PostInput(existingPost.Blog.Name, post["title"] as string,
                                                post["description"] as string, post["mt_excerpt"] as string,
                                                (post["categories"] as object[]).OfType<string>() ??
                                                Enumerable.Empty<string>(),
                                                string.IsNullOrEmpty(post["mt_basename"] as string)
                                                    ? expressions.Slugify(post["title"] as string)
                                                    : post["mt_basename"] as string,
                                                publish
                                                    ? existingPost.Published.HasValue
                                                          ? existingPost.Published.Value
                                                          : DateTime.UtcNow
                                                    : (DateTime?) null,
                                                existingPost.CommentingDisabled);

            ModelResult<Post> results = postService.EditPost(new PostAddress(existingPost.Blog.Name, existingPost.Slug), postInput, EntityState.Normal, new ServiceContext(HttpContext, RouteData, user));

            if (results.IsValid)
                return new XmlRpcResult(true);

            return new XmlRpcFaultResult(0, results.GetFirstException().Message);
        }

        [ActionName("metaWeblog.getPost")]
        public ActionResult GetPost(string postId, string username, string password)
        {
            UserAuthenticated user = Get_user(username, password);
            Post post = metaWeblogPostService.GetPost(new MetaWeblogPostAddress(new Guid(postId)), new ServiceContext(HttpContext, new RouteData(), user));

            if (post == null) throw new ArgumentOutOfRangeException();

            return new XmlRpcResult(ModelPostToServicePost(post));
        }

        //TODO: (erikpo) Need to implement this method if the current setup supports writing to the file system
        [ActionName("metaWeblog.newMediaObject")]
        public ActionResult NewMediaObject(string blogId, string username, string password, IDictionary<string, object> file)
        {
            return new XmlRpcFaultResult(0, "Not Implemented");
        }

        [ActionName("metaWeblog.getCategories")]
        public ActionResult GetCategories(string blogId, string username, string password)
        {
            Get_user(username, password);

            return new XmlRpcResult(tagService.GetTags().Select(t => new Dictionary<string, object>{{"description", t.Name}, {"htmlUrl", string.Empty}, {"rssUrl", string.Empty}}).ToArray());
        }

        [ActionName("metaWeblog.getRecentPosts")]
        public ActionResult GetRecentPosts(string blogId, string username, string password, int numberOfPosts)
        {
            UserAuthenticated user = Get_user(username, password);

            return new XmlRpcResult(postService.GetPostsWithDrafts(0, numberOfPosts, new BlogAddress(blogId), new ServiceContext(HttpContext, new RouteData(), user)).Select(p => ModelPostToServicePost(p)).ToArray());
        }

        [ActionName("blogger.getUsersBlogs")]
        public ActionResult GetUsersBlogs(string apikey, string username, string password)
        {
            Get_user(username, password);

            //TODO: (erikpo) Make this return only the list of blogs the current user has explicit access to
            return new XmlRpcResult(blogService.GetBlogs(0, 10000).Select(b => new Dictionary<string, object>{{"url", string.Empty}, {"blogid", b.Name}, {"blogName", b.DisplayName}}).ToArray());
        }

        [ActionName("blogger.deletePost")]
        public ActionResult DeletePost(string appkey, string postid, string username, string password, bool publish)
        {
            UserAuthenticated user = Get_user(username, password);

            metaWeblogPostService.RemovePost(new MetaWeblogPostAddress(new Guid(postid)), new ServiceContext(HttpContext, new RouteData(), user));

            return new XmlRpcResult(true);
        }

        public ContentResult LiveWriterManifest()
        {
            XNamespace ns = "http://schemas.microsoft.com/wlw/manifest/weblog";

            //TODO: (erikpo) Make these settings dynamic based on capabilities

            XDocument doc =
                new XDocument(
                    new XDeclaration("1.0", "utf-8", "no"),
                    new XElement(
                        ns + "manifest",
                        new XElement(
                            ns + "options",
                            new XElement[]
                            {
                                new XElement(ns + "clientType", "Metaweblog"),
                                new XElement(ns + "supportsExcerpt", "Yes"),
                                new XElement(ns + "supportsNewCategories", "Yes"),
                                new XElement(ns + "supportsNewCategoriesInline", "Yes"),
                                new XElement(ns + "requiresHtmlTitles", "No"),
                                new XElement(ns + "requiresXHTML", "Yes"),
                                new XElement(ns + "supportsScripts", "Yes"),
                                new XElement(ns + "supportsEmbeds", "Yes"),
                                new XElement(ns + "supportsSlug", "Yes"),
                                new XElement(ns + "supportsFileUpload", "No")
                            }
                            )
                        )
                    );

            return Content(doc.ToString(), "text/xml");
        }

        private UserAuthenticated Get_user(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Invalid login");

            UserAuthenticated user = userService.GetUser(username, password);

            if (user != null)
                return user;

            throw new InvalidCredentialException();
        }

        private static string saltAndHash(string rawString, string salt)
        {
            byte[] salted = Encoding.UTF8.GetBytes(string.Concat(rawString, salt));
            SHA256 hasher = new SHA256Managed();
            byte[] hashed = hasher.ComputeHash(salted);

            return Convert.ToBase64String(hashed);
        }

        private static IDictionary<string, object> ModelPostToServicePost(Post post)
        {
            return new Dictionary<string, object>
            {
                { "categories", post.Tags.Select(t => t.Name).ToArray() },
                { "dateCreated", post.Created },
                { "description", post.Body },
                { "mt_basename", post.Slug },
                { "mt_excerpt", post.BodyShort },
                { "postid", post.ID.ToString() },
                { "title", post.Title },
                { "userid", post.Creator.ID.ToString() },
            };
        }
    }
}
