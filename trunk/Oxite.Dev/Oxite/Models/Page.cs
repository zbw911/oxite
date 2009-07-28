//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using Oxite.Infrastructure;

namespace Oxite.Models
{
    public class Page : EntityBase, INamedEntity, ICacheItem
    {
        public Page(Guid siteID, Guid id)
            : base(id)
        {
            Site = new SiteSmall(siteID);
        }

        public Page(Guid siteID, Guid id, Page parent, string slug)
            : this(siteID, id, parent, true, slug)
        {
        }

        public Page(Guid siteID, Guid id, Page parent, bool hasChildren, UserAuthenticated creator, string title, string body, string slug, EntityState state, DateTime created, DateTime modified, DateTime? published)
            : this(siteID, id, parent, hasChildren, slug)
        {
            HasChildren = hasChildren;
            Creator = creator;
            Title = title;
            Body = body;
            Slug = slug;
            State = state;
            Created = created;
            Modified = modified;
            Published = published;
        }

        public Page(string body, UserAuthenticated creator, Guid siteID, Guid id, Guid parentPageID, DateTime? published, string slug, EntityState state, string title)
            : this(siteID, id)
        {
            Body = body;
            Creator = creator;
            Parent = new Page(siteID, parentPageID);
            Published = published;
            Slug = slug;
            State = state;
            Title = title;
        }

        private Page(Guid siteID, Guid id, Page parent, bool hasChildren, string slug)
            : this(siteID, id)
        {
            Parent = parent;
            HasChildren = hasChildren;
            Slug = slug;
        }

        public SiteSmall Site { get; private set; }
        public Page Parent { get; private set; }
        public UserAuthenticated Creator { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public EntityState State { get; private set; }
        public string Slug { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Modified { get; private set; }
        public DateTime? Published { get; private set; }
        public bool HasChildren { get; private set; }

        private string path;
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = string.Format(
                        "{0}/{1}",
                        Parent != null && !string.IsNullOrEmpty(Parent.Path) && Parent.Path != "/"
                            ? Parent.Path
                            : "",
                        Slug
                        );
                }

                return path;
            }
        }

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

            dependencies.Add(Site);

            return dependencies;
        }

        #endregion
    }
}
