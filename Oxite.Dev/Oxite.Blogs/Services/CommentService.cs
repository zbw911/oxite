//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
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
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository repository;
        private readonly IPostRepository postRepository;
        private readonly ILanguageRepository languageRepository;
        private readonly ILocalizationRepository localizationRepository;
        private readonly ITrackbackOutboundRepository trackbackOutboundRepository;
        private readonly IMessageOutboundRepository messageOutboundRepository;
        private readonly AbsolutePathHelper absolutePathHelper;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ISiteService siteService;
        private readonly IOxiteCacheModule cache;

        public CommentService(ICommentRepository repository, IPostRepository postRepository, ILanguageRepository languageRepository, ILocalizationRepository localizationRepository, ITrackbackOutboundRepository trackbackOutboundRepository, IMessageOutboundRepository messageOutboundRepository, AbsolutePathHelper absolutePathHelper, IValidationService validator, IPluginEngine pluginEngine, ISiteService siteService, IModulesLoaded modules)
        {
            this.repository = repository;
            this.postRepository = postRepository;
            this.languageRepository = languageRepository;
            this.localizationRepository = localizationRepository;
            this.trackbackOutboundRepository = trackbackOutboundRepository;
            this.messageOutboundRepository = messageOutboundRepository;
            this.absolutePathHelper = absolutePathHelper;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.siteService = siteService;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region ICommentService Members

        public IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, bool includePending, bool sortDescending, ServiceContext context)
        {
            Guid siteID = getSite().ID;

            return cache.GetItems<IPageOfItems<Comment>, Comment>(
                string.Format("GetComments-IncludePending:{0},SortDescending{1}", includePending, sortDescending),
                new CachePartition(pageIndex, pageSize),
                () => processDisplayOfComments(() => repository.GetComments(siteID, includePending, sortDescending).GetPage(pageIndex, pageSize), context),
                c => getCommentDependencies(c, new SiteSmall(siteID))
                );
        }

        public IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Blog blog, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Comment>, Comment>(
                string.Format("GetComments-Blog:{0:N}", blog.ID),
                new CachePartition(pageIndex, pageSize),
                () => processDisplayOfComments(() => repository.GetCommentsByBlog(blog.ID).GetPage(pageIndex, pageSize), context),
                c => getCommentDependencies(c, blog)
                );
        }

        public IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Post post, bool includeUnapproved, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Comment>, Comment>(
                string.Format("GetComments-Post:{0:N},IncludeUnapproved:{1}", post.ID, includeUnapproved),
                new CachePartition(pageIndex, pageSize),
                () => processDisplayOfComments(() => repository.GetCommentsByPost(post.ID, includeUnapproved).GetPage(pageIndex, pageSize), context),
                c => getCommentDependencies(c, post)
                );
        }

        public IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Tag tag, ServiceContext context)
        {
            return cache.GetItems<IPageOfItems<Comment>, Comment>(
                string.Format("GetComments-Tag:{0:N}", tag.ID),
                new CachePartition(pageIndex, pageSize),
                () => processDisplayOfComments(() => repository.GetCommentsByTag(getSite().ID, tag.ID).GetPage(pageIndex, pageSize), context),
                c => getCommentDependencies(c, tag)
                );
        }

        public ValidationStateDictionary ValidateCommentInput(CommentInput commentInput)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(CommentInput), validator.Validate(commentInput));

            return validationState;
        }

        private string generateUniqueCommentSlug(PostAddress postAddress)
        {
            string commentSlug = null;
            bool isUnique = false;

            while (!isUnique)
            {
                commentSlug = Guid.NewGuid().ToString("N").Substring(0, 5);

                Comment foundComment = getComment(new CommentAddress(postAddress, commentSlug));

                isUnique = foundComment == null;
            }

            return commentSlug;
        }

        public ModelResult<Comment> AddComment(PostAddress postAddress, CommentInput commentInput, ServiceContext context)
        {
            PluginCommentInput pluginCommentInput = new PluginCommentInput(commentInput);

            pluginEngine.ExecuteAll("ProcessInputOfComment", new { context, comment = pluginCommentInput });
            commentInput = pluginCommentInput.ToCommentInput();
            
            commentInput = pluginEngine.Process<PluginCommentInput>("ProcessInputOfCommentOnAdd", new PluginCommentInput(commentInput)).ToCommentInput();

            if (pluginEngine.AnyTrue("IsCommentSpam", new { context, comment = pluginCommentInput }))
                return new ModelResult<Comment>(new ValidationStateDictionary(typeof(CommentInput), new ValidationState(new [] { new ValidationError("Comment.IsSpam", commentInput, "The supplied comment was considered to be spam and was not added") })));

            ValidationStateDictionary validationState = ValidateCommentInput(commentInput);

            if (!validationState.IsValid) return new ModelResult<Comment>(validationState);

            Site site = getSite();
            EntityState commentState;

            try
            {
                commentState = context.CurrentUser != null ? EntityState.Normal : (EntityState)Enum.Parse(typeof(EntityState), site.CommentStateDefault);
            }
            catch
            {
                commentState = EntityState.PendingApproval;
            }

            //TODO: (erikpo) Replace with some logic to set the language from the user's browser or from a dropdown list
            Language language = languageRepository.GetLanguage(site.LanguageDefault ?? "en");
            Comment comment;

            using (TransactionScope transaction = new TransactionScope())
            {
                string commentSlug = generateUniqueCommentSlug(postAddress);

                comment = commentInput.ToComment(context.CurrentUser, context.HttpContext.Request.GetUserIPAddress().ToLong(), context.HttpContext.Request.UserAgent, language, commentSlug, commentState);

                comment = repository.Save(comment, site.ID, postAddress.BlogName, postAddress.PostSlug);

                if (comment.State == EntityState.Normal)
                    invalidateCachedCommentDependencies(comment);

                transaction.Complete();
            }

            //TODO: (erikpo) Move into a module
            if (commentInput.Subscribe)
            {
                Guid siteID = getSite().ID;

                if (context.CurrentUser != null)
                {
                    if (!postRepository.GetSubscriptionExists(siteID, comment.Post.BlogName, comment.Post.Slug, comment.CreatorUserID))
                        postRepository.AddSubscription(siteID, comment.Post.BlogName, comment.Post.Slug, comment.CreatorUserID);
                }
                else
                {
                    if (!postRepository.GetSubscriptionExists(siteID, comment.Post.BlogName, comment.Post.Slug, comment.CreatorEmail))
                        postRepository.AddSubscription(siteID, comment.Post.BlogName, comment.Post.Slug, comment.CreatorName, comment.CreatorEmail);
                }
            }

            //TODO: (erikpo) Move into a module
            messageOutboundRepository.Save(generateMessages(comment.Post, comment));

            pluginEngine.ExecuteAll("CommentAdded", new { context, comment = new CommentReadOnly(comment) });

            if (comment.State == EntityState.Normal)
                pluginEngine.ExecuteAll("CommentApproved", new { context, comment = new CommentReadOnly(comment) });

            return new ModelResult<Comment>(comment, validationState);
        }

        public ModelResult<Comment> AddComment(Post post, ImportCommentInput commentInput, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(ImportCommentInput), validator.Validate(commentInput));

            if (!validationState.IsValid) return new ModelResult<Comment>(validationState);

            Guid siteID = getSite().ID;
            Comment comment;

            using (TransactionScope transaction = new TransactionScope())
            {
                string commentSlug = generateUniqueCommentSlug(post.ToPostAddress());

                comment = commentInput.ToComment(commentSlug);

                comment = repository.Save(comment, siteID, post.Blog.Name, post.Slug);

                invalidateCachedCommentDependencies(comment);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("CommentAddedFromImport", new { context, comment = new CommentReadOnly(comment) });

            return new ModelResult<Comment>(comment, validationState);
        }

        public ModelResult<Comment> EditComment(CommentAddress commentAddress, CommentInput commentInput, ServiceContext context)
        {
            commentInput = pluginEngine.Process<PluginCommentInput>("ProcessInputOfComment", new PluginCommentInput(commentInput)).ToCommentInput();
            commentInput = pluginEngine.Process<PluginCommentInput>("ProcessInputOfCommentOnEdit", new PluginCommentInput(commentInput)).ToCommentInput();

            if (pluginEngine.AnyTrue("IsCommentSpam", commentInput))
                return new ModelResult<Comment>(new ValidationStateDictionary(typeof(CommentInput), new ValidationState(new[] { new ValidationError("Comment.IsSpam", commentInput, "The supplied comment was considered to be spam and was not added") })));

            ValidationStateDictionary validationState = ValidateCommentInput(commentInput);

            if (!validationState.IsValid) return new ModelResult<Comment>(validationState);

            Guid siteID = getSite().ID;
            Comment newComment;
            Comment originalComment;

            using (TransactionScope transaction = new TransactionScope())
            {
                originalComment = getComment(commentAddress);
                newComment = originalComment.Apply(commentInput, context.CurrentUser);

                newComment = repository.Save(newComment, siteID, newComment.Post.BlogName, newComment.Post.Slug);

                invalidateCachedCommentForEdit(newComment, originalComment);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("CommentEdited", new { context, comment = new CommentReadOnly(newComment), commentOriginal = new CommentReadOnly(originalComment) });

            return new ModelResult<Comment>(newComment, validationState);
        }

        public bool RemoveComment(CommentAddress commentAddress, ServiceContext context)
        {
            return changeState(commentAddress, EntityState.Removed, "CommentRemoved", context);
        }

        public bool ApproveComment(CommentAddress commentAddress, ServiceContext context)
        {
            return changeState(commentAddress, EntityState.Normal, "CommentApproved", context);
        }

        private bool changeState(CommentAddress commentAddress, EntityState state, string pluginEventName, ServiceContext context)
        {
            bool commentStateChanged = false;
            Comment comment;

            using (TransactionScope transaction = new TransactionScope())
            {
                comment = getComment(commentAddress);

                if (comment != null && comment.State != state)
                {
                    repository.ChangeState(comment.ID, state);

                    comment = repository.GetComment(comment.ID);

                    commentStateChanged = comment.State == state;
                }

                if (commentStateChanged)
                    cache.InvalidateItem(comment);

                transaction.Complete();
            }

            if (commentStateChanged)
                pluginEngine.ExecuteAll(pluginEventName, new { context, comment = new CommentReadOnly(comment) });

            return commentStateChanged;
        }

        #endregion

        #region Private Methods

        private Site getSite()
        {
            return siteService.GetSite();
        }

        private void invalidateCachedCommentDependencies(Comment comment)
        {
            if (comment.Parent != null)
                cache.InvalidateItem(new Comment(comment.Parent.ID));

            cache.InvalidateItem(new Post(comment.Post.ID));
        }

        private void invalidateCachedCommentForEdit(Comment newComment, Comment originalComment)
        {
            if (originalComment.Parent != null && newComment.Parent == null)
                cache.InvalidateItem(new Comment(originalComment.Parent.ID));
            else if (originalComment.Parent == null && newComment.Parent != null)
                cache.InvalidateItem(new Comment(newComment.Parent.ID));

            if (originalComment.Post.ID != newComment.Post.ID)
            {
                cache.InvalidateItem(new Post(originalComment.Post.ID));
                cache.InvalidateItem(new Post(newComment.Post.ID));
            }

            cache.InvalidateItem(newComment);
        }

        private void invalidateCachedCommentForRemove(Comment comment)
        {
            invalidateCachedCommentDependencies(comment);

            cache.InvalidateItem(comment);
        }

        private IEnumerable<ICacheItem> getCommentDependencies(Comment comment, ICacheItem dependency)
        {
            List<ICacheItem> dependencies = new List<ICacheItem>();

            if (comment != null)
            {
                dependencies.Add(new Post(comment.Post.ID));

                dependencies.Add(comment);
            }
            else
                dependencies.Add(dependency);

            return dependencies;
        }

        private IPageOfItems<Comment> processDisplayOfComments(Func<IPageOfItems<Comment>> getComments, ServiceContext serviceContext)
        {
            IPageOfItems<Comment> comments = getComments();
            List<Comment> newComments = new List<Comment>();

            foreach (Comment comment in comments)
            {
                PluginComment commentProxy = new PluginComment(comment);

                pluginEngine.ExecuteAll("ProcessDisplayOfComment", new { context = serviceContext, comment = commentProxy });

                newComments.Add(commentProxy.ToComment());
            }

            return comments;
        }

        private Comment getComment(CommentAddress commentAddress)
        {
            return repository.GetComment(getSite().ID, commentAddress.BlogName, commentAddress.PostSlug, commentAddress.CommentSlug);
        }

        private IEnumerable<MessageOutbound> generateMessages(PostSmall post, Comment comment)
        {
            Site site = getSite();
            IEnumerable<PostSubscription> subscriptions = postRepository.GetSubscriptions(site.ID, post.BlogName, post.Slug);
            List<MessageOutbound> messages = new List<MessageOutbound>();
            //TODO: (erikpo) Once the plugin model is done, get this from the plugin
            int retryCount = 4;

            foreach (PostSubscription subscription in subscriptions)
            {
                string userName = subscription.UserName;

                MessageOutbound message = new MessageOutbound
                {
                    ID = Guid.NewGuid(),
                    To = !string.IsNullOrEmpty(userName) ? string.Format("{0} <{1}>", userName, subscription.UserEmail) : subscription.UserEmail,
                    Subject = string.Format(getPhrase("Messages.Formats.ReplySubject", site.LanguageDefault, "RE: {0}"), post.Title),
                    Body = generateMessageBody(post, comment, site),
                    RemainingRetryCount = retryCount
                };

                messages.Add(message);
            }

            return messages;
        }

        private string generateMessageBody(PostSmall post, Comment comment, Site site)
        {
            string body = getPhrase("Messages.NewComment", site.LanguageDefault, getDefaultBody());
            //TODO: (erikpo) Change this to come from the user this message is going to if applicable
            double timeZoneOffset = site.TimeZoneOffset;

            body = body.Replace("{Site.Name}", site.DisplayName);
            body = body.Replace("{User.Name}", comment.CreatorName);
            body = body.Replace("{Post.Title}", post.Title);
            //TODO: (erikpo) Change the published date to be relative (e.g. 5 minutes ago)
            body = body.Replace("{Comment.Created}", comment.Created.AddHours(timeZoneOffset).ToLongTimeString());
            body = body.Replace("{Comment.Body}", comment.Body);
            body = body.Replace("{Comment.Permalink}", absolutePathHelper.GetAbsolutePath(comment).Replace("%23", "#"));

            return body;
        }

        private static string getDefaultBody()
        {
            return
                "<h1>New Comment on {Site.Name}</h1>" +
                "<h2>{User.Name} commented on '{Post.Title}' at {Comment.Created}</h2>" +
                "<p>{Comment.Body}</p>" +
                "<a href=\"{Comment.Permalink}\">{Comment.Permalink}</a>";
        }

        private string getPhrase(string key, string language)
        {
            return getPhrase(key, language, null);
        }

        private string getPhrase(string key, string language, string defaultValue)
        {
            Phrase phrase = localizationRepository.GetPhrases().Where(p => p.Key == key && p.Language == language).FirstOrDefault();

            if (phrase != null)
                return phrase.Value;

            if (defaultValue == null)
                return key;

            return defaultValue;
        }

        #endregion
    }
}
