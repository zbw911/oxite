using System;
//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Linq;
using Oxite.Models;
using Oxite.Modules.Search.Models;

namespace Oxite.Modules.Search.Repositories.SqlServer
{
    public class SqlServerSearchPostRepository : ISearchPostRepository
    {
        private readonly OxiteSearchDataContext context;

        public SqlServerSearchPostRepository(OxiteSearchDataContext context)
        {
            this.context = context;
        }

        #region ISearchPostRepository Members

        public IQueryable<Post> GetPosts(Guid siteID, SearchCriteria criteria)
        {
            //TODO: (erikpo) Should call a stored procedure instead for full text searches (maybe a plugin or module?)
            IQueryable<oxite_Post> query =
                from b in context.oxite_Blogs
                join p in context.oxite_Posts on b.BlogID equals p.BlogID
                where b.SiteID == siteID && (p.Title.Contains(criteria.Term) || p.Body.Contains(criteria.Term) || p.Slug.Contains(criteria.Term)) && p.PublishedDate != null && p.PublishedDate < DateTime.UtcNow
                select p;

            return projectPosts(query);
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
                   where p.State != (byte)EntityState.Removed
                   orderby p.PublishedDate descending
                   select projectPost(p, u, b, c, t);
        }

        private static Post projectPost(oxite_Post p, oxite_User u, oxite_Blog b, IQueryable<oxite_Comment> c, IQueryable<Tag> t)
        {
            Blog blog = new Blog(b.SiteID, b.CommentingDisabled, b.CreatedDate, b.Description, b.DisplayName, b.BlogID, b.ModifiedDate, b.BlogName);

            UserAuthenticated creator =
                u != null
                ? new UserAuthenticated(u.UserID, u.Username, u.DisplayName, u.Email, u.HashedEmail, new Language(u.oxite_Language.LanguageID) { Name = u.oxite_Language.LanguageName, DisplayName = u.oxite_Language.LanguageDisplayName }, (EntityState)u.Status)
                : null;

            return new Post(blog, p.Body, p.BodyShort, p.CommentingDisabled, p.CreatedDate, creator, p.PostID, c.Count(), p.ModifiedDate, p.PublishedDate, p.Slug, (EntityState)p.State, t.ToList(), p.Title, Enumerable.Empty<Trackback>());
        }

        private IQueryable<oxite_Comment> getCommentsQuery(Guid postID)
        {
            return
                from c in context.oxite_Comments
                where c.PostID == postID && c.State == (byte)EntityState.Normal
                orderby c.CreatedDate ascending
                select c;
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

        #endregion
    }
}
