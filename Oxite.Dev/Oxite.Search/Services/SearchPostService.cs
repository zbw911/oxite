//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Blogs.Extensions;
using Oxite.Models.Extensions;
using Oxite.Modules.Search.Models;
using Oxite.Modules.Search.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Services;

namespace Oxite.Modules.Search.Services
{
    public class SearchPostService : ISearchPostService
    {
        private readonly Guid siteID;
        private readonly ISearchPostRepository repository;
        private readonly IPluginEngine pluginEngine;
        private readonly IOxiteCacheModule cache;

        public SearchPostService(ISiteService siteService, ISearchPostRepository repository, IPluginEngine pluginEngine, IModulesLoaded modules)
        {
            this.siteID = siteService.GetSite().ID;
            this.repository = repository;
            this.pluginEngine = pluginEngine;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region ISearchPostService Members

        public IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, SearchCriteria criteria, ServiceContext context)
        {
            IPageOfItems<Post> posts =
                cache.GetItems<IPageOfItems<Post>, Post>(
                    string.Format("GetPosts-SearchTerm:{0}", criteria.Term),
                    new CachePartition(pageIndex, pageSize),
                    () => pluginEngine.ProcessDisplayOfPosts(context, () => repository.GetPosts(siteID, criteria).GetPage(pageIndex, pageSize)),
                    p => p.GetPostDependencies()
                    );

            if (context.RequestDataFormat.IsFeed())
                posts = posts.Since(context.HttpContext.Request.IfModifiedSince());

            pluginEngine.ExecuteAll("UserSearched", new { context, criteria });

            return posts;
        }

        #endregion
    }
}
