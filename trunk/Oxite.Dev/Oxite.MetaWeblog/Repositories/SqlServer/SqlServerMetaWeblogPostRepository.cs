//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.MetaWeblog.Repositories.SqlServer
{
    public class SqlServerMetaWeblogPostRepository : IMetaWeblogPostRepository
    {
        private readonly OxiteMetaWeblogDataContext context;

        public SqlServerMetaWeblogPostRepository(OxiteMetaWeblogDataContext context)
        {
            this.context = context;
        }

        #region IMetaWeblogPostRepository Members

        public Post GetPost(Guid id)
        {
            IQueryable<oxite_Post> post =
                from p in context.oxite_Posts
                where p.PostID == id
                select p;

            return projectPosts(post).FirstOrDefault();
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

        #endregion

        #region Private Methods

        private IQueryable<Post> projectPosts(IQueryable<oxite_Post> posts)
        {
            return from p in posts
                   join u in context.oxite_Users on p.CreatorUserID equals u.UserID
                   join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                   let t = getTagsQuery(p.PostID)
                   where p.State != (byte)EntityState.Removed
                   orderby p.PublishedDate descending
                   select projectPost(p, u, b, t);
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

        private static Post projectPost(oxite_Post p, oxite_User u, oxite_Blog b, IQueryable<Tag> t)
        {
            Blog blog = new Blog(b.SiteID, b.CommentingDisabled, b.CreatedDate, b.Description, b.DisplayName, b.BlogID, b.ModifiedDate, b.BlogName);

            UserAuthenticated creator =
                u != null
                ? new UserAuthenticated(u.UserID, u.Username, u.DisplayName, u.Email, u.HashedEmail, new Language(u.oxite_Language.LanguageID) { Name = u.oxite_Language.LanguageName, DisplayName = u.oxite_Language.LanguageDisplayName }, (EntityState)u.Status)
                : null;

            return new Post(blog, p.Body, p.BodyShort, p.CommentingDisabled, p.CreatedDate, creator, p.PostID, 0, p.ModifiedDate, p.PublishedDate, p.Slug, (EntityState)p.State, t.ToList(), p.Title, Enumerable.Empty<Trackback>());
        }

        #endregion
    }
}
