//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;
using Oxite.Repositories;

namespace Oxite.Modules.Blogs.Repositories.SqlServer
{
    public class SqlServerTagRepository : ITagRepository
    {
        private readonly OxiteBlogsDataContext context;
        private readonly Guid siteID;

        public SqlServerTagRepository(OxiteBlogsDataContext context, Site site)
        {
            this.context = context;
            this.siteID = site.ID;
        }

        #region ITagRepository Members

        public IQueryable<Tag> GetTags()
        {
            return from t in context.oxite_Tags
                   join pt in context.oxite_Tags on t.ParentTagID equals pt.TagID
                   select new Tag
                   {
                       Created = t.CreatedDate,
                       ID = t.TagID,
                       Name = t.TagName,
                       DisplayName = pt.TagName
                   };
        }

        public IQueryable<KeyValuePair<Tag, int>> GetTagsWithPostCount()
        {
            return from tt in
                       (from t in context.oxite_Tags
                        join ptr in context.oxite_PostTagRelationships on t.TagID equals ptr.TagID
                        join p in context.oxite_Posts on ptr.PostID equals p.PostID
                        where p.State == (byte)EntityState.Normal && p.PublishedDate <= DateTime.Now.ToUniversalTime()
                        select new { Tag = t.oxite_Tag1, Post = p })
                   group tt by tt.Tag into results
                   where results.Key.TagID == results.Key.ParentTagID
                   orderby results.Key.TagName
                   select new KeyValuePair<Tag, int>(
                       new Tag
                       {
                           Created = results.Key.CreatedDate,
                           ID = results.Key.TagID,
                           Name = results.Key.TagName,
                           DisplayName = results.Key.TagName
                       },
                       results.Count()
                       );
        }

        public Tag GetTag(string tagName)
        {
            return (from t in context.oxite_Tags
                    where t.TagName == tagName
                    select new Tag
                    {
                        ID = t.TagID,
                        Name = t.TagName,
                        Created = t.CreatedDate,
                        DisplayName = t.TagName
                    }).FirstOrDefault();
        }

        public IQueryable<KeyValuePair<Tag, int>> GetTagsUsedInBlog(string blogName)
        {
            return from tt in
                       (from t in context.oxite_Tags
                        join ptr in context.oxite_PostTagRelationships on t.TagID equals ptr.TagID
                        join p in context.oxite_Posts on ptr.PostID equals p.PostID
                        join b in context.oxite_Blogs on p.BlogID equals b.BlogID
                        where b.SiteID == siteID && string.Compare(b.BlogName, blogName, true) == 0 && p.State == (byte)EntityState.Normal && p.PublishedDate <= DateTime.Now.ToUniversalTime() 
                        select new { Tag = t.oxite_Tag1, Post = p })
                   group tt by tt.Tag into results
                   where results.Key.TagID == results.Key.ParentTagID
                   orderby results.Key.TagName
                   select new KeyValuePair<Tag, int>(
                       new Tag
                       {
                           Created = results.Key.CreatedDate,
                           ID = results.Key.TagID,
                           Name = results.Key.TagName,
                           DisplayName = results.Key.TagName
                       },
                       results.Count()
                       );
        }

        #endregion
    }
}
