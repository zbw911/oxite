//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Repositories
{
    public interface IPostRepository
    {
        IQueryable<Post> GetPosts(Guid siteID, bool includeDrafts);
        IQueryable<Post> GetPostsByTag(Guid siteID, string tagName);
        IQueryable<Post> GetPostsByBlogWithDrafts(Guid siteID, string blogName);
        IQueryable<Post> GetPostsByBlog(Guid siteID, string blogName);
        IQueryable<Post> GetPostsByArchive(Guid siteID, int year, int month, int day);
        IQueryable<Post> GetPostsByFileType(Guid siteID, string typeName);
        IEnumerable<Post> GetPosts(Guid siteID, DateTime startDate, DateTime endDate);
        IEnumerable<DateTime> GetPostDateGroups(Guid siteID);
        Post GetPost(Guid siteID, string blogName, string slug);
        Post Save(Post post);
        bool Remove(Guid postID);
        void RemoveAllByBlog(Guid siteID, string blogName);

        IQueryable<KeyValuePair<ArchiveData, int>> GetArchives(Guid siteID);
        IQueryable<KeyValuePair<ArchiveData, int>> GetArchivesByBlog(Guid siteID, string blogName);

        IEnumerable<PostSubscription> GetSubscriptions(Guid siteID, string blogName, string postSlug);
        bool GetSubscriptionExists(Guid siteID, string blogName, string postSlug, Guid userID);
        bool GetSubscriptionExists(Guid siteID, string blogName, string postSlug, string userEmail);
        void AddSubscription(Guid siteID, string blogName, string postSlug, Guid userID);
        void AddSubscription(Guid siteID, string blogName, string postSlug, string userName, string userEmail);

        void SaveTrackback(Post post, Trackback trackback);
    }
}
