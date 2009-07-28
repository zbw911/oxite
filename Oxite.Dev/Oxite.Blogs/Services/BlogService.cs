﻿//  --------------------------------
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
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Plugins.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Blogs.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository repository;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ISiteService siteService;
        private readonly IOxiteCacheModule cache;

        public BlogService(IBlogRepository repository, IValidationService validator, IPluginEngine pluginEngine, ISiteService siteService, IModulesLoaded modules)
        {
            this.repository = repository;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.siteService = siteService;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region IBlogService Members

        public bool GetBlogExists(string blogName)
        {
            return cache.GetItem<bool?>(
                string.Format("GetBlogExists-Name:{0}", blogName),
                () => repository.GetBlog(getSiteID(), blogName) != null,
                null
                ).GetValueOrDefault(false);
        }

        public Blog GetBlog(BlogAddress blogAddress)
        {
            return cache.GetItem<Blog>(
                string.Format("GetBlog-Name:{0}", blogAddress.BlogName),
                () => repository.GetBlog(getSiteID(), blogAddress.BlogName),
                a => getBlogDependencies(a)
                );
        }

        public IPageOfItems<Blog> GetBlogs(int pageIndex, int pageSize)
        {
            return repository.GetBlogs(getSiteID()).GetPage(pageIndex, pageSize);
        }

        public IEnumerable<Blog> FindBlogs(BlogSearchCriteria criteria)
        {
            return
                repository.GetBlogs(getSiteID()).ToArray()
                .Where(a => 
                    a.Name.IndexOf(criteria.Name, StringComparison.OrdinalIgnoreCase) != -1 ||
                    a.DisplayName.IndexOf(criteria.Name, StringComparison.OrdinalIgnoreCase) != -1
                    ).ToArray();
        }

        private void validateBlog(Guid siteID, Blog newBlog, ValidationStateDictionary validationState)
        {
            validateBlog(siteID, newBlog, newBlog, validationState);
        }

        private void validateBlog(Guid siteID, Blog newBlog, Blog originalBlog, ValidationStateDictionary validationState)
        {
            ValidationState state = new ValidationState();
            Blog foundBlog;

            validationState.Add(typeof(Blog), state);

            foundBlog = repository.GetBlog(siteID, newBlog.Name);

            if (foundBlog != null && newBlog.Name != originalBlog.Name)
                state.Errors.Add(new ValidationError("Blogs.NameNotUnique", newBlog.Name, "A blog already exists with the supplied name"));
        }

        private ModelResult<Blog> editBlog<T>(BlogAddress blogAddress, T input, Func<Blog, Blog> applyInput, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(T), validator.Validate(input));

            if (!validationState.IsValid) return new ModelResult<Blog>(validationState);

            Guid siteID = getSiteID();
            Blog originalBlog;
            Blog newBlog;

            using (TransactionScope transaction = new TransactionScope())
            {
                originalBlog = repository.GetBlog(siteID, blogAddress.BlogName);
                newBlog = applyInput(originalBlog);

                validateBlog(siteID, newBlog, originalBlog, validationState);

                if (!validationState.IsValid) return new ModelResult<Blog>(validationState);

                newBlog = repository.Save(newBlog);

                invalidateCachedBlogForEdit(newBlog, originalBlog);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("BlogEdited", new { context, blog = new BlogReadOnly(newBlog), blogOriginal = new BlogReadOnly(originalBlog) });

            return new ModelResult<Blog>(newBlog, validationState);
        }

        public ModelResult<Blog> AddBlog(BlogInput blogInput, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(BlogInput), validator.Validate(blogInput));

            if (!validationState.IsValid) return new ModelResult<Blog>(validationState);

            Guid siteID = getSiteID();
            Blog blog;

            using (TransactionScope transaction = new TransactionScope())
            {
                blog = blogInput.ToBlog(siteID);

                validateBlog(siteID, blog, validationState);

                if (!validationState.IsValid) return new ModelResult<Blog>(validationState);

                blog = repository.Save(blog);

                invalidateCachedBlogDependencies(blog);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("BlogAdded", new { context, blog = new BlogReadOnly(blog) });

            return new ModelResult<Blog>(blog, validationState);
        }

        public ModelResult<Blog> EditBlog(BlogAddress blogAddress, BlogInput blogInput, ServiceContext context)
        {
            return editBlog(blogAddress, blogInput, b => b.Apply(blogInput), context);
        }

        public ModelResult<Blog> EditBlog(BlogAddress blogAddress, ImportBlogInput blogInput, ServiceContext context)
        {
            return editBlog(blogAddress, blogInput, b => b.Apply(blogInput), context);
        }

        public void RemoveBlog(BlogAddress blogAddress, ServiceContext context)
        {
            Guid siteID = getSiteID();

            using (TransactionScope transaction = new TransactionScope())
            {
                Blog blog = repository.GetBlog(siteID, blogAddress.BlogName);

                if (blog != null && repository.Remove(siteID, blogAddress.BlogName))
                {
                    invalidateCachedBlogForRemove(blog);

                    transaction.Complete();

                    pluginEngine.ExecuteAll("BlogRemoved", new { context, blog = new BlogReadOnly(blog) });

                    return;
                }

                transaction.Complete();
            }
        }

        #endregion

        #region Private Methods

        private Guid getSiteID()
        {
            return siteService.GetSite().ID;
        }

        private IEnumerable<ICacheItem> getBlogDependencies(Blog blog)
        {
            List<ICacheItem> dependencies = new List<ICacheItem>();

            dependencies.Add(blog.Site);

            dependencies.Add(blog);

            return dependencies;
        }

        private void invalidateCachedBlogDependencies(Blog blog)
        {
            cache.InvalidateItem(blog.Site);

            cache.Invalidate(string.Format("GetBlogExists-Name:{0}", blog.Name));
        }

        private void invalidateCachedBlogForEdit(Blog newBlog, Blog originalBlog)
        {
            cache.InvalidateItem(newBlog);
        }

        private void invalidateCachedBlogForRemove(Blog blog)
        {
            invalidateCachedBlogDependencies(blog);

            cache.InvalidateItem(blog);
        }

        #endregion
    }
}
