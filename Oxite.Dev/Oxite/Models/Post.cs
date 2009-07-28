//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Infrastructure;

namespace Oxite.Models
{
    public class Post : EntityBase, INamedEntity, ICacheItem
    {
        public Post(Guid id)
            : base(id)
        {
        }

        public Post(Blog blog, string body, string bodyShort, bool commentingDisabled, UserAuthenticated creator, DateTime? published, string slug, EntityState state, IEnumerable<Tag> tags, string title)
        {
            Blog = blog;
            Body = body;
            BodyShort = bodyShort;
            CommentingDisabled = commentingDisabled;
            Creator = creator;
            Published = published;
            Slug = slug;
            State = state;
            Tags = tags;
            Title = title;
        }

        public Post(Blog blog, string body, string bodyShort, bool commentingDisabled, DateTime created, UserAuthenticated creator, Guid id, int commentCount, DateTime modified, DateTime? published, string slug, EntityState state, IEnumerable<Tag> tags, string title, IEnumerable<Trackback> trackbacks)
            : this(id)
        {
            Blog = blog;
            Body = body;
            BodyShort = bodyShort;
            CommentingDisabled = commentingDisabled;
            Creator = creator;
            Published = published;
            Slug = slug;
            State = state;
            Tags = tags;
            Title = title;
            Created = created;
            CommentCount = commentCount;
            Modified = modified;
            Trackbacks = trackbacks;
        }

        public Blog Blog { get; private set; }
        public string Body { get; private set; }
        public string BodyShort { get; private set; }
        public int CommentCount { get; private set; }
        public bool CommentingDisabled { get; private set; }
        public DateTime Created { get; private set; }
        public UserAuthenticated Creator { get; private set; }
        public DateTime Modified { get; private set; }
        public DateTime? Published { get; private set; }
        public string Slug { get; private set; }
        public EntityState State { get; private set; }
        public IEnumerable<Tag> Tags { get; private set; }
        public string Title { get; private set; }
        public IEnumerable<Trackback> Trackbacks { get; private set; }

        #region INamedEntity Members

        string INamedEntity.Name
        {
            get { return Slug; }
        }

        string INamedEntity.DisplayName
        {
            get { return Title; }
        }

        #endregion

        #region ICacheItem Members

        string ICacheItem.GetCacheItemKey()
        {
            return base.GetCacheItemKey();
        }

        IEnumerable<ICacheItem> ICacheItem.GetCacheDependencyItems()
        {
            List<ICacheItem> dependencies = new List<ICacheItem>();

            if (Blog != null)
                dependencies.Add(Blog);

            if (Tags != null)
                dependencies.AddRange(Tags.Cast<ICacheItem>());

            return dependencies;
        }

        #endregion
    }
}
