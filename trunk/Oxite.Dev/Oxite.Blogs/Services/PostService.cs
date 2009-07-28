//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Models.Extensions;
using Oxite.Modules.Blogs.Extensions;
using Oxite.Modules.Blogs.Infrastructure;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Plugins.Models;
using Oxite.Repositories;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Blogs.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository repository;
        private readonly ILocalizationRepository localizationRepository;
        private readonly ITrackbackOutboundRepository trackbackOutboundRepository;
        private readonly IMessageOutboundRepository messageOutboundRepository;
        private readonly AbsolutePathHelper absolutePathHelper;
        private readonly IRegularExpressions expressions;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ISiteService siteService;
        private readonly IOxiteCacheModule cache;

        public PostService(IPostRepository repository, ILocalizationRepository localizationRepository, ITrackbackOutboundRepository trackbackOutboundRepository, IMessageOutboundRepository messageOutboundRepository, AbsolutePathHelper absolutePathHelper, IRegularExpressions expressions, IValidationService validator, IPluginEngine pluginEngine, ISiteService siteService, IModulesLoaded modules)
        {
            this.repository = repository;
            this.localizationRepository = localizationRepository;
            this.trackbackOutboundRepository = trackbackOutboundRepository;
            this.messageOutboundRepository = messageOutboundRepository;
            this.absolutePathHelper = absolutePathHelper;
            this.expressions = expressions;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.siteService = siteService;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region IPostService Members

        public IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, ServiceContext context)
        {
            bool includeDrafts = context.RequestDataFormat == RequestDataFormat.Web && context.CurrentUser != null && context.CurrentUser.IsInRole("Admin");
            IPageOfItems<Post> posts =
                cache.GetItems<IPageOfItems<Post>, Post>(
                    "GetPosts",
                    new CachePartition(pageIndex, pageSize),
                    () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPosts(getSite().ID, includeDrafts).GetPage(pageIndex, pageSize)),
                    p => p.GetPostDependencies()
                    );

            if (!includeDrafts)
                posts = posts.Since(context.HttpContext.Request.IfModifiedSince());

            return posts;
        }

        public IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, TagAddress tagAddress, ServiceContext context)
        {
            IPageOfItems<Post> posts =
                cache.GetItems<IPageOfItems<Post>, Post>(
                    string.Format("GetPosts-Tag:{0}", tagAddress.TagName),
                    new CachePartition(pageIndex, pageSize),
                    () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPostsByTag(getSite().ID, tagAddress.TagName).GetPage(pageIndex, pageSize)),
                    p => p.GetPostDependencies()
                    );

            if (context.RequestDataFormat.IsFeed())
                posts = posts.Since(context.HttpContext.Request.IfModifiedSince());

            return posts;
        }

        public IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, BlogAddress blogAddress, ServiceContext context)
        {
            IPageOfItems<Post> posts =
                cache.GetItems<IPageOfItems<Post>, Post>(
                    string.Format("GetPosts-Blog:{0}", blogAddress.BlogName),
                    new CachePartition(pageIndex, pageSize),
                    () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPostsByBlog(getSite().ID, blogAddress.BlogName).GetPage(pageIndex, pageSize)),
                    p => p.GetPostDependencies()
                    );

            if (context.RequestDataFormat.IsFeed())
                posts = posts.Since(context.HttpContext.Request.IfModifiedSince());

            return posts;
        }

        public IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, ArchiveData archive, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Post>, Post>(
                string.Format("GetPosts-Year:{0},Month:{1},Day:{2}", archive.Year, archive.Month, archive.Day),
                new CachePartition(pageIndex, pageSize),
                () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPostsByArchive(getSite().ID, archive.Year, archive.Month, archive.Day).GetPage(pageIndex, pageSize)),
                p => p.GetPostDependencies()
                );
        }

        public IPageOfItems<Post> GetPostsWithDrafts(int pageIndex, int pageSize, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Post>, Post>(
                "GetPostsWithDrafts",
                new CachePartition(pageIndex, pageSize),
                () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPosts(getSite().ID, true).GetPage(pageIndex, pageSize)),
                p => p.GetPostDependencies()
                );
        }

        public IPageOfItems<Post> GetPostsWithDrafts(int pageIndex, int pageSize, BlogAddress blogAddress, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Post>, Post>(
                string.Format("GetPostsWithDrafts-Blog:{0}", blogAddress.BlogName),
                new CachePartition(pageIndex, pageSize),
                () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPostsByBlogWithDrafts(getSite().ID, blogAddress.BlogName).GetPage(pageIndex, pageSize)),
                p => p.GetPostDependencies()
                );
        }

        public IPageOfItems<Post> GetPostsByFileType(int pageIndex, int pageSize, string typeName, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Post>, Post>(
                string.Format("GetPostsWithDrafts-FileType:{0}", typeName),
                new CachePartition(pageIndex, pageSize),
                () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPostsByFileType(getSite().ID, typeName).GetPage(pageIndex, pageSize)),
                p => p.GetPostDependencies()
                );
        }

        public IEnumerable<Post> GetPosts(DateRangeAddress dateRangeAddress, ServiceContext context)
        {
            return cache.GetItems<IEnumerable<Post>, Post>(
                string.Format("GetPostsWithDrafts-StartDate:{0},EndDate:{1}", dateRangeAddress.StartDate, dateRangeAddress.EndDate),
                () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPosts(getSite().ID, dateRangeAddress.StartDate, dateRangeAddress.EndDate)),
                p => p.GetPostDependencies()
                );
        }

        //TODO: (erikpo) Need to change the query to return back data about the posts so they can be added as cache dependencies
        public IEnumerable<DateTime> GetPostDateGroups(ServiceContext context)
        {
            return cache.GetItems<IEnumerable<DateTime>, DateTime>(
                "GetPostDateGroups",
                () => repository.GetPostDateGroups(getSite().ID),
                null
                );
        }

        public Post GetPost(PostAddress postAddress, ServiceContext context)
        {
            return cache.GetItem<Post>(
                string.Format("GetPost-Blog:{0},Post:{1}", postAddress.BlogName, postAddress.PostSlug),
                () => pluginEngine.ProcessDisplayOfPost(context, () => repository.GetPost(getSite().ID, postAddress.BlogName, postAddress.PostSlug)),
                p => p.GetPostDependencies()
                );
        }

        public Post GetRandomPost()
        {
            IQueryable<Post> posts = repository.GetPosts(getSite().ID, false);
            Random rnd = new Random(0);
            int randomRowIndex = rnd.Next(posts.Count() - 1);

            return posts.Skip(randomRowIndex).Take(1).FirstOrDefault();
        }

        public ValidationStateDictionary ValidatePostInput(PostInput postInput)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(PostInput), validator.Validate(postInput));

            return validationState;
        }

        private void validatePost(Guid siteID, Post newPost, ValidationStateDictionary validationState)
        {
            validatePost(siteID, newPost, newPost, validationState);
        }

        private void validatePost(Guid siteID, Post newPost, Post originalPost, ValidationStateDictionary validationState)
        {
            ValidationState state = new ValidationState();
            Post foundPost;

            validationState.Add(typeof(Post), state);

            foundPost  = repository.GetPost(siteID, newPost.Blog.Name, newPost.Slug);

            if (foundPost != null && (newPost.Blog.ID != originalPost.Blog.ID || newPost.Slug != originalPost.Slug))
                state.Errors.Add("Post.SlugNotUnique", newPost.Slug, "A post already exists with the supplied blog and slug");
        }

        private ModelResult<Post> addPost<T>(T postInput, Func<Post> generatePost)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(T), validator.Validate(postInput));

            if (!validationState.IsValid) return new ModelResult<Post>(validationState);

            Guid siteID = getSite().ID;
            Post post;

            using (TransactionScope transaction = new TransactionScope())
            {
                post = generatePost();

                validatePost(siteID, post, validationState);

                if (!validationState.IsValid) return new ModelResult<Post>(validationState);

                post = repository.Save(post);

                invalidateCachedPostDependencies(post);

                transaction.Complete();
            }

            return new ModelResult<Post>(post, validationState);
        }

        private void addCreatorSubscription(Post post)
        {
            Site site = getSite();

            if (site.AuthorAutoSubscribe && !repository.GetSubscriptionExists(site.ID, post.Blog.Name, post.Slug, post.Creator.ID))
                repository.AddSubscription(site.ID, post.Blog.Name, post.Slug, post.Creator.ID);
        }

        public ModelResult<Post> AddPost(PostInput postInput, EntityState postState, ServiceContext context)
        {
            postInput = pluginEngine.Process<PluginPostInput>("ProcessInputOfPost", new PluginPostInput(postInput)).ToPostInput();
            postInput = pluginEngine.Process<PluginPostInput>("ProcessInputOfPostOnAdd", new PluginPostInput(postInput)).ToPostInput();
            postInput = postInput.NormalizeTags(t => expressions.Clean("TagReplace", t));

            ModelResult<Post> results = addPost(postInput, () => postInput.ToPost(postState, context.CurrentUser));

            if (results.IsValid)
            {
                Post newPost = results.Item;

                //TODO: (erikpo) Move this into a module
                addCreatorSubscription(newPost);

                //TODO: (erikpo) Move this into a module
                if (newPost.State == EntityState.Normal && newPost.Published.HasValue)
                {
                    string postUrl = absolutePathHelper.GetAbsolutePath(newPost);
                    IEnumerable<TrackbackOutbound> trackbacksToAdd = extractTrackbacks(newPost, postUrl, newPost.Blog.DisplayName);
                    IEnumerable<TrackbackOutbound> unsentTrackbacks = trackbackOutboundRepository.GetUnsent(newPost.ID);
                    IEnumerable<TrackbackOutbound> trackbacksToRemove = trackbacksToAdd.Where(tb => !unsentTrackbacks.Contains(tb) && !tb.Sent.HasValue);

                    trackbackOutboundRepository.Remove(trackbacksToRemove);
                    trackbackOutboundRepository.Save(trackbacksToAdd);
                }
                else
                {
                    //TODO: (erikpo) Remove all outbound trackbacks
                }

                pluginEngine.ExecuteAll("PostSaved", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });
                pluginEngine.ExecuteAll("PostAdded", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });

                if (newPost.State == EntityState.Normal && newPost.Published.HasValue && newPost.Published.Value <= DateTime.UtcNow)
                    pluginEngine.ExecuteAll("PostPublished", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });
            }

            return results;
        }

        public ModelResult<Post> AddPost(Blog blog, ImportPostInput postInput, ServiceContext context)
        {
            ModelResult<Post> results = addPost(postInput.NormalizeTags(t => expressions.Clean("TagReplace", t)), () => postInput.ToPost(blog));

            if (results.IsValid)
            {
                Post newPost = results.Item;

                pluginEngine.ExecuteAll("PostAddedFromImport", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });

                //TODO: (erikpo) Move this into a module
                addCreatorSubscription(newPost);
            }

            return results;
        }

        public ModelResult<Post> EditPost(PostAddress postAddress, PostInput postInput, EntityState postState, ServiceContext context) 
        {
            postInput = pluginEngine.Process<PluginPostInput>("ProcessInputOfPost", new PluginPostInput(postInput)).ToPostInput();
            postInput = pluginEngine.Process<PluginPostInput>("ProcessInputOfPostOnEdit", new PluginPostInput(postInput)).ToPostInput();
            postInput = postInput.NormalizeTags(t => expressions.Clean("TagReplace", t));

            ValidationStateDictionary validationState = ValidatePostInput(postInput);

            if (!validationState.IsValid) return new ModelResult<Post>(validationState);

            Guid siteID = getSite().ID;
            Post newPost;
            Post originalPost;
            bool isPublished;

            using (TransactionScope transaction = new TransactionScope())
            {
                originalPost = repository.GetPost(getSite().ID, postAddress.BlogName, postAddress.PostSlug);
                isPublished = originalPost.State == EntityState.Normal && originalPost.Published.HasValue && originalPost.Published.Value <= DateTime.UtcNow;
                newPost = originalPost.Apply(postInput, postState, context.CurrentUser);

                validatePost(siteID, newPost, originalPost, validationState);

                if (!validationState.IsValid) return new ModelResult<Post>(validationState);

                newPost = repository.Save(newPost);

                invalidateCachedPostForEdit(newPost, originalPost);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("PostSaved", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });
            pluginEngine.ExecuteAll("PostEdited", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)), postOriginal = new PostReadOnly(originalPost, absolutePathHelper.GetAbsolutePath(originalPost)) });

            bool isNowPublished = newPost.State == EntityState.Normal && newPost.Published.HasValue && newPost.Published.Value <= DateTime.UtcNow;
            if (!isPublished && isNowPublished)
                pluginEngine.ExecuteAll("PostPublished", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });
            else if (isPublished && !isNowPublished)
                pluginEngine.ExecuteAll("PostUnpublished", new { context, post = new PostReadOnly(newPost, absolutePathHelper.GetAbsolutePath(newPost)) });

            return new ModelResult<Post>(newPost, validationState);
        }

        public void RemovePost(PostAddress postAddress, ServiceContext context)
        {
            if (postAddress == null) throw new ArgumentNullException("postAddress");

            Guid siteID = getSite().ID;

            using (TransactionScope transaction = new TransactionScope())
            {
                Post post = repository.GetPost(siteID, postAddress.BlogName, postAddress.PostSlug);

                if (post != null)
                {
                    if (repository.Remove(post.ID))
                    {
                        invalidateCachedPostForRemove(post);

                        transaction.Complete();

                        pluginEngine.ExecuteAll("PostRemoved", new { context, post = new PostReadOnly(post, absolutePathHelper.GetAbsolutePath(post)) });

                        return;
                    }
                }
            }
        }

        public void RemoveAll(BlogAddress blogAddress)
        {
            repository.RemoveAllByBlog(getSite().ID, blogAddress.BlogName);
        }

        //TODO: (erikpo) Need to change the query to return back data about the posts so they can be added as cache dependencies
        public IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives()
        {
            return cache.GetItems<IEnumerable<KeyValuePair<ArchiveData, int>>, KeyValuePair<ArchiveData, int>>(
                "GetArchives",
                () => repository.GetArchives(getSite().ID).ToArray(),
                null
                );
        }

        //TODO: (erikpo) Need to change the query to return back data about the posts so they can be added as cache dependencies
        public IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives(BlogAddress blogAddress)
        {
            return cache.GetItems<IEnumerable<KeyValuePair<ArchiveData, int>>, KeyValuePair<ArchiveData, int>>(
                string.Format("GetArchives-Blog:{0}", blogAddress.BlogName),
                () => repository.GetArchivesByBlog(getSite().ID, blogAddress.BlogName).ToArray(),
                null
                );
        }

        public ValidationStateDictionary AddTrackback(Post post, Trackback trackback)
        {
            repository.SaveTrackback(post, trackback);

            return null;
        }

        public ValidationStateDictionary EditTrackback(Trackback trackback)
        {
            //post.Trackbacks.Add(trackback);

            //repository.Save(post);

            return null;
        }

        #endregion

        #region Private Methods

        private Site getSite()
        {
            return siteService.GetSite();
        }

        private void invalidateCachedPostDependencies(Post post)
        {
            cache.InvalidateItem(post.Blog);

            post.Tags.ToList().ForEach(t => cache.InvalidateItem(t));
        }

        private void invalidateCachedPostForEdit(Post newPost, Post originalPost)
        {
            // if the blog has changed on the post then invalidate the original post are and the new post blog
            if (originalPost.Blog.ID != newPost.Blog.ID)
            {
                cache.InvalidateItem(originalPost.Blog);
                cache.InvalidateItem(newPost.Blog);
            }

            // invalidate any tags that have changed since the edit
            originalPost.Tags.Except(newPost.Tags).Union(newPost.Tags.Except(originalPost.Tags)).ToList().ForEach(t => cache.InvalidateItem(t));

            cache.InvalidateItem(newPost);
        }

        private void invalidateCachedPostForRemove(Post post)
        {
            invalidateCachedPostDependencies(post);

            cache.InvalidateItem(post);
        }

        private static IEnumerable<TrackbackOutbound> extractTrackbacks(Post post, string postUrl, string postBlogTitle)
        {
            //INFO: (erikpo) Trackback spec: http://www.sixapart.com/pronet/docs/trackback_spec
            Regex r =
                new Regex(
                    @"(?<HTML><a[^>]*href\s*=\s*[\""\']?(?<HRef>[^""'>\s]*)[\""\']?[^>]*>(?<Title>[^<]+|.*?)?</a>)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled
                    );
            MatchCollection m = r.Matches(post.Body);
            List<TrackbackOutbound> trackbacks = Enumerable.Empty<TrackbackOutbound>().ToList();

            if (m.Count > 0)
            {
                //TODO: (erikpo) Once the plugin model is done, get this from the plugin
                const int retryCount = 28;

                trackbacks = new List<TrackbackOutbound>(m.Count);

                foreach (Match match in m)
                {
                    trackbacks.Add(
                        new TrackbackOutbound
                        {
                            TargetUrl = match.Groups["HRef"].Value,
                            PostID = post.ID,
                            PostTitle = post.Title,
                            PostBody = post.GetBodyShort(),
                            PostBlogTitle = postBlogTitle,
                            PostUrl = postUrl,
                            RemainingRetryCount = retryCount
                        }
                        );
                }
            }

            return trackbacks;
        }

        #endregion
    }
}
