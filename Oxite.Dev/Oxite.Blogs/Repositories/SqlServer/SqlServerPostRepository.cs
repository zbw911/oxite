//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Repositories.SqlServer
{
    public class SqlServerPostRepository : IPostRepository
    {
        private readonly OxiteBlogsDataContext context;

        public SqlServerPostRepository(OxiteBlogsDataContext context)
        {
            this.context = context;
        }

        #region IPostRepository Members

        public IQueryable<Post> GetPosts(Guid siteID, bool includeDrafts)
        {
            if (includeDrafts)
                return projectPosts(getPostsBySiteQuery(siteID));

            return projectPosts(excludeNotYetPublished(getPostsBySiteQuery(siteID)));
        }

        public IQueryable<Post> GetPostsByTag(Guid siteID, string tagName)
        {
            return projectPosts(excludeNotYetPublished(getPostsByTagQuery(siteID, tagName)));
        }

        private IQueryable<oxite_Post> getPostsByTagQuery(Guid siteID, string tagName)
        {
            return
                from p in context.oxite_Posts
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                join ptr in context.oxite_PostTagRelationships on p.PostID equals ptr.PostID
                join t in context.oxite_Tags on ptr.TagID equals t.TagID
                where b.SiteID == siteID && string.Compare(t.TagName, tagName, true) == 0
                select p;
        }

        public IQueryable<Post> GetPostsByBlogWithDrafts(Guid siteID, string blogName)
        {
            return projectPosts(getPostsByBlogQuery(siteID, blogName));
        }

        public IQueryable<Post> GetPostsByBlog(Guid siteID, string blogName)
        {
            return projectPosts(excludeNotYetPublished(getPostsByBlogQuery(siteID, blogName)));
        }

        private IQueryable<oxite_Post> getPostsByBlogQuery(Guid siteID, string blogName)
        {
            return
                from p in context.oxite_Posts
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0
                select p;
        }

        public IQueryable<Post> GetPostsByFileType(Guid siteID, string typeName)
        {
            var query =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                join f in context.oxite_Files on p.PostID equals f.PostID
                where b.SiteID == siteID && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal && string.Compare(f.TypeName, typeName, true) == 0
                orderby p.PublishedDate
                select p;

            return projectPosts(query);
        }

        public IEnumerable<Post> GetPosts(Guid siteID, DateTime startDate, DateTime endDate)
        {
            return projectPosts(
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && p.PublishedDate != null && p.PublishedDate >= startDate && p.PublishedDate < endDate && p.State == (byte)EntityState.Normal
                orderby p.PublishedDate
                select p
                ).ToArray();
        }

        public IEnumerable<DateTime> GetPostDateGroups(Guid siteID)
        {
            return (
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                orderby p.PublishedDate
                group p by new DateTime(p.PublishedDate.Value.Year, p.PublishedDate.Value.Month, 1)
                    into results
                    select results.Key
                ).ToArray();
        }

        public IQueryable<KeyValuePair<ArchiveData, int>> GetArchives(Guid siteID)
        {
            return
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                let month = new DateTime(p.PublishedDate.Value.Year, p.PublishedDate.Value.Month, 1)
                group p by month into months
                orderby months.Key descending
                select new KeyValuePair<ArchiveData, int>(new ArchiveData(months.Key.Year + "/" + months.Key.Month), months.Count());
        }

        public IQueryable<KeyValuePair<ArchiveData, int>> GetArchivesByBlog(Guid siteID, string blogName)
        {
            return
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal
                let month = new DateTime(p.PublishedDate.Value.Year, p.PublishedDate.Value.Month, 1)
                group p by month into months
                orderby months.Key descending
                select new KeyValuePair<ArchiveData, int>(new ArchiveData(months.Key.Year + "/" + months.Key.Month), months.Count());
        }

        public Post Save(Post post)
        {
            oxite_Post postToSave = null;

            if (post.ID != Guid.Empty)
                postToSave = context.oxite_Posts.FirstOrDefault(p => p.PostID == post.ID);

            if (postToSave == null)
            {
                postToSave = new oxite_Post();

                postToSave.PostID = post.ID != Guid.Empty ? post.ID : Guid.NewGuid();
                postToSave.CreatedDate = postToSave.ModifiedDate = DateTime.UtcNow;

                context.oxite_Posts.InsertOnSubmit(postToSave);
            }
            else
                postToSave.ModifiedDate = DateTime.UtcNow;

            postToSave.Body = post.Body;
            postToSave.BodyShort = post.BodyShort;
            postToSave.PublishedDate = post.Published;
            postToSave.Slug = post.Slug;
            postToSave.State = (byte)post.State;
            postToSave.Title = post.Title;
            postToSave.CommentingDisabled = post.CommentingDisabled;

            // Tags: Use existing, create new ones if needed. Don't edit old tags
            foreach (Tag tag in post.Tags)
            {
                oxite_Tag persistenceTag = context.oxite_Tags.Where(t => t.TagName.ToLower() == tag.Name.ToLower()).FirstOrDefault();

                if (persistenceTag == null)
                {
                    Guid newTagID = Guid.NewGuid();
                    persistenceTag = new oxite_Tag { TagName = tag.Name, CreatedDate = tag.Created.HasValue ? tag.Created.Value : DateTime.UtcNow, TagID = newTagID, ParentTagID = newTagID };
                    context.oxite_Tags.InsertOnSubmit(persistenceTag);
                }

                if (!context.oxite_PostTagRelationships.Where(pt => pt.PostID == postToSave.PostID && pt.TagID == persistenceTag.TagID).Any())
                    context.oxite_PostTagRelationships.InsertOnSubmit(new oxite_PostTagRelationship { PostID = postToSave.PostID, TagID = persistenceTag.TagID, TagDisplayName = tag.DisplayName ?? tag.Name });
            }

            var tagsRemoved = from t in context.oxite_Tags
                              join pt in context.oxite_PostTagRelationships on t.TagID equals pt.TagID
                              where pt.PostID == postToSave.PostID && !post.Tags.Select(tag => tag.Name.ToLower()).Contains(t.TagName.ToLower())
                              select pt;

            context.oxite_PostTagRelationships.DeleteAllOnSubmit(tagsRemoved);

            if (post.Blog == null || string.IsNullOrEmpty(post.Blog.Name)) throw new InvalidOperationException("No blog was specified");
            oxite_Blog blog = context.oxite_Blogs.FirstOrDefault(b => string.Compare(b.BlogName, post.Blog.Name, true) == 0);
            if (blog == null) throw new InvalidOperationException(string.Format("Blog {0} could not be found", post.Blog.Name));
            postToSave.BlogID = blog.BlogID;

            oxite_User user = context.oxite_Users.FirstOrDefault(u => u.Username == post.Creator.Name);
            if (user == null) throw new InvalidOperationException(string.Format("User {0} could not be found", post.Creator.Name));
            postToSave.CreatorUserID = user.UserID;

            postToSave.SearchBody = postToSave.Title + string.Join("", post.Tags.Select(t => t.Name + t.DisplayName).ToArray()) + postToSave.Body + user.DisplayName + user.Username;

            context.SubmitChanges();

            return getPost(postToSave.PostID);
        }

        public bool Remove(Guid postID)
        {
            oxite_Post foundPost = context.oxite_Posts.FirstOrDefault(p => p.PostID == postID);
            bool removedPost = false;

            if (foundPost != null)
            {
                foundPost.State = (byte)EntityState.Removed;

                context.SubmitChanges();

                removedPost = true;
            }

            return removedPost;
        }

        public IQueryable<Post> GetPostsByArchive(Guid siteID, int year, int month, int day)
        {
            var query =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && p.PublishedDate != null && p.PublishedDate <= DateTime.UtcNow && p.State == (byte)EntityState.Normal && p.PublishedDate.Value.Year == year
                select p;

            if (month > 0)
                query = query.Where(p => p.PublishedDate.Value.Month == month);

            if (day > 0)
                query = query.Where(p => p.PublishedDate.Value.Day == day);

            return projectPosts(query);
        }

        public Post GetPost(Guid siteID, string blogName, string slug)
        {
            IQueryable<oxite_Post> post =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && string.Compare(p.Slug, slug, true) == 0
                select p;

            return projectPosts(post).FirstOrDefault();
        }

        private Post getPost(Guid postID)
        {
            IQueryable<oxite_Post> post =
                from p in context.oxite_Posts
                where p.PostID == postID
                select p;

            return projectPosts(post).FirstOrDefault();
        }

        //INFO: (erikpo) Not sure if this logic should exist here or in the database (as cascade delete on the relationships)
        public void RemoveAllByBlog(Guid siteID, string blogName)
        {
            var posts =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0
                select p;

            var postTagRelationships =
                from p in posts
                join ptr in context.oxite_PostTagRelationships on p.PostID equals ptr.PostID
                select ptr;

            var postComments =
                from p in posts
                join c in context.oxite_Comments on p.PostID equals c.PostID
                select c;

            var postSubscriptions =
                from p in posts
                join s in context.oxite_Subscriptions on p.PostID equals s.PostID
                select s;

            var postTrackbacks =
                from p in posts
                join t in context.oxite_Trackbacks on p.PostID equals t.PostID
                select t;

            context.oxite_Trackbacks.DeleteAllOnSubmit(postTrackbacks);
            context.oxite_Subscriptions.DeleteAllOnSubmit(postSubscriptions);
            context.oxite_Comments.DeleteAllOnSubmit(postComments);
            context.oxite_PostTagRelationships.DeleteAllOnSubmit(postTagRelationships);
            context.oxite_Posts.DeleteAllOnSubmit(posts);

            context.SubmitChanges();
        }

        public IEnumerable<PostSubscription> GetSubscriptions(Guid siteID, string blogName, string postSlug)
        {
            var query =
                from s in context.oxite_Subscriptions
                join p in context.oxite_Posts on s.PostID equals p.PostID
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                join u in context.oxite_Users on s.UserID equals u.UserID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug
                select new { Blog = b, Post = p, User = u, Subscription = s };

            //TOOD: (erikpo) Need to figure out why this query fails if not .ToArray()'d before selecting.  LINQ to SQL isn't liking something in the select, but not sure what.
            var temp = query.ToArray();

            return temp.Select(t => new PostSubscription(
                    t.Subscription.SubscriptionID,
                    new PostSmall(t.Post.PostID, t.Blog.BlogName, t.Post.Slug, t.Post.Title),
                    t.User.Username != "Anonymous" ? !string.IsNullOrEmpty(t.User.DisplayName) ? t.User.DisplayName : t.User.Username : t.Subscription.UserName,
                    t.User.Username != "Anonymous" ? t.User.Email : t.Subscription.UserEmail
                    )
                );
        }

        public bool GetSubscriptionExists(Guid siteID, string blogName, string postSlug, Guid userID)
        {
            return (
                from s in context.oxite_Subscriptions
                join p in context.oxite_Posts on s.PostID equals p.PostID
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug && s.UserID == userID
                select s
                ).Any();
        }

        public bool GetSubscriptionExists(Guid siteID, string blogName, string postSlug, string userEmail)
        {
            Guid userID = context.oxite_Users.Single(u => u.Username == "Anonymous").UserID;

            return (
                from s in context.oxite_Subscriptions
                join p in context.oxite_Posts on s.PostID equals p.PostID
                join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug && s.UserID == userID && s.UserEmail == userEmail
                select s
                ).Any();
        }

        private void addSubscription(Guid siteID, string blogName, string postSlug, Action<oxite_Subscription> setSubscription)
        {
            Guid postID = (from p in context.oxite_Posts join b in context.oxite_Blogs on p.BlogID equals b.BlogID where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.Slug == postSlug select p).Single().PostID;
            oxite_Subscription subscription;

            context.oxite_Subscriptions.InsertOnSubmit(
                subscription = new oxite_Subscription
                {
                    PostID = postID,
                    SubscriptionID = Guid.NewGuid()
                }
                );

            setSubscription(subscription);
            
            context.SubmitChanges();
        }

        public void AddSubscription(Guid siteID, string blogName, string postSlug, Guid userID)
        {
            addSubscription(siteID, blogName, postSlug, s => s.UserID = userID);
        }

        public void AddSubscription(Guid siteID, string blogName, string postSlug, string userName, string userEmail)
        {
            addSubscription(siteID, blogName, postSlug, s => setSubscriptionAnonymous(s, userName, userEmail));
        }

        private void setSubscriptionAnonymous(oxite_Subscription subscription, string userName, string userEmail)
        {
            subscription.UserID = context.oxite_Users.Single(u => u.Username == "Anonymous").UserID;
            subscription.UserName = userName;
            subscription.UserEmail = userEmail;
        }

        public void SaveTrackback(Post post, Trackback trackback)
        {
            oxite_Trackback persistenceTrackback = new oxite_Trackback
            {
                BlogName = trackback.BlogName,
                Body = trackback.Body,
                CreatedDate = trackback.Created ?? DateTime.UtcNow,
                IsTargetInSource = trackback.IsTargetInSource,
                ModifiedDate = DateTime.UtcNow,
                PostID = post.ID,
                Source = trackback.Source,
                Title = trackback.Title,
                TrackbackID = Guid.NewGuid(),
                Url = trackback.Url
            };

            context.oxite_Trackbacks.InsertOnSubmit(persistenceTrackback);

            context.SubmitChanges();
        }

        #endregion

        #region Private Methods

        private IQueryable<Post> projectPosts(IQueryable<oxite_Post> posts)
        {
            return from p in posts
                   join u in context.oxite_Users on p.CreatorUserID equals u.UserID
                   join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                   let c = getCommentsQuery(p.PostID)
                   let t = getTagsQuery(p.PostID)
                   let tb = getTrackbacksQuery(p.PostID)
                   where p.State != (byte)EntityState.Removed
                   orderby p.PublishedDate descending
                   select projectPost(p, u, b, c, t, tb);
        }

        private static Post projectPost(oxite_Post p, oxite_User u, oxite_Blog b, IQueryable<oxite_Comment> c, IQueryable<Tag> t, IQueryable<Trackback> tb)
        {
            Blog blog = new Blog(b.SiteID, b.CommentingDisabled, b.CreatedDate, b.Description, b.DisplayName, b.BlogID, b.ModifiedDate, b.BlogName);

            UserAuthenticated creator =
                u != null
                ? new UserAuthenticated(u.UserID, u.Username, u.DisplayName, u.Email, u.HashedEmail, new Language(u.oxite_Language.LanguageID) { Name = u.oxite_Language.LanguageName, DisplayName = u.oxite_Language.LanguageDisplayName }, (EntityState)u.Status)
                : null;

            return new Post(blog, p.Body, p.BodyShort, p.CommentingDisabled, p.CreatedDate, creator, p.PostID, c.Count(), p.ModifiedDate, p.PublishedDate, p.Slug, (EntityState)p.State, t.ToList(), p.Title, tb.ToList());
        }

        private IQueryable<Trackback> getTrackbacksQuery(Guid postID)
        {
            return from tb in context.oxite_Trackbacks
                   where tb.PostID == postID
                   select new Trackback
                   {
                       BlogName = tb.BlogName,
                       Body = tb.Body,
                       Created = tb.CreatedDate,
                       ID = tb.TrackbackID,
                       IsTargetInSource = tb.IsTargetInSource,
                       Modified = tb.ModifiedDate,
                       Source = tb.Source,
                       Title = tb.Title,
                       Url = tb.Url
                   };
        }

        private IQueryable<Tag> getTagsQuery(Guid guid)
        {
            return from t in context.oxite_Tags
                   join pt in context.oxite_PostTagRelationships on t.TagID equals pt.TagID
                   where pt.PostID == guid
                   select new Tag
                   {
                       Created = t.CreatedDate,
                       ID = t.TagID,
                       Name = t.TagName,
                       DisplayName = pt.TagDisplayName
                   };
        }

        private IQueryable<oxite_Comment> getCommentsQuery(Guid postID)
        {
            return
                from c in context.oxite_Comments
                where c.PostID == postID && c.State == (byte)EntityState.Normal
                orderby c.CreatedDate ascending
                select c;
        }

        private IQueryable<oxite_Post> getPostsBySiteQuery(Guid siteID)
        {
            return
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID
                select p;
        }

        private static IQueryable<oxite_Post> excludeNotYetPublished(IQueryable<oxite_Post> query)
        {
            return query.Where(p => p.PublishedDate != null && p.PublishedDate < DateTime.UtcNow);
        }

        #endregion
    }
}
