//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Blogs.Extensions;
using Oxite.Modules.Blogs.Infrastructure;
using Oxite.Modules.MetaWeblog.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Plugins.Models;
using Oxite.Services;

namespace Oxite.Modules.MetaWeblog.Services
{
    public class MetaWeblogPostService : IMetaWeblogPostService
    {
        private readonly IMetaWeblogPostRepository repository;
        private readonly IOxiteCacheModule cache;
        private readonly IPluginEngine pluginEngine;
        private readonly AbsolutePathHelper absolutePathHelper;

        public MetaWeblogPostService(IMetaWeblogPostRepository repository, IModulesLoaded modules, IPluginEngine pluginEngine, AbsolutePathHelper absolutePathHelper)
        {
            this.repository = repository;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
            this.pluginEngine = pluginEngine;
            this.absolutePathHelper = absolutePathHelper;
        }

        #region IMetaWeblogPostService Members

        public Post GetPost(MetaWeblogPostAddress postAddress, ServiceContext context)
        {
            return cache.GetItem<Post>(
                string.Format("GetPost-Post:{0:N}", postAddress.PostID),
                () => pluginEngine.ProcessDisplayOfPost(context, () => repository.GetPost(postAddress.PostID)),
                p => p.GetPostDependencies()
                );
        }

        public void RemovePost(MetaWeblogPostAddress postAddress, ServiceContext context)
        {
            if (postAddress == null) throw new ArgumentNullException("postAddress");
            if (postAddress.PostID == Guid.Empty) throw new ArgumentException("PostID must be set");

            using (TransactionScope transaction = new TransactionScope())
            {
                Post post = repository.GetPost(postAddress.PostID);

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

        #endregion

        #region Private Methods

        private void invalidateCachedPostDependencies(Post post)
        {
            cache.InvalidateItem(post.Blog);

            post.Tags.ToList().ForEach(t => cache.InvalidateItem(t));
        }

        private void invalidateCachedPostForRemove(Post post)
        {
            invalidateCachedPostDependencies(post);

            cache.InvalidateItem(post);
        }

        #endregion
    }
}
