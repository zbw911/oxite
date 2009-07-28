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
using Oxite.Modules.CMS.Extensions;
using Oxite.Modules.CMS.Models;
using Oxite.Modules.CMS.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Plugins.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.CMS.Services
{
    public class PageService : IPageService
    {
        private readonly IPageRepository repository;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ISiteService siteService;
        private readonly IOxiteCacheModule cache;

        public PageService(IPageRepository repository, IValidationService validator, IPluginEngine pluginEngine, ISiteService siteService, IModulesLoaded modules)
        {
            this.repository = repository;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.siteService = siteService;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region IPageService Members

        public Page GetPage(PageAddress pageAddress, ServiceContext context)
        {
            Page page = null;
            Guid parentPageID = Guid.Empty;
            List<ICacheItem> parents = new List<ICacheItem>();

            foreach (PagePartAddress pagePartAddress in pageAddress.PagesInPath)
            {
                page = cache.GetItem<Page>(
                    string.Format("GetPage-ParentID:{0:N},Slug:{1}", parentPageID, pagePartAddress.PagePartName),
                    () => processDisplayOfPage(() => repository.GetPage(getSiteID(), pagePartAddress.PagePartName, parentPageID), context),
                    p => parents
                    );

                if (page != null)
                {
                    parentPageID = page.ID;
                    parents.Add(page);
                }
                else
                    break;
            }

            return page;
        }

        public IEnumerable<Page> GetPages()
        {
            return repository.GetPages(getSiteID()).ToArray();
        }

        public ValidationStateDictionary ValidatePageInput(PageInput pageInput)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(PageInput), validator.Validate(pageInput));

            return validationState;
        }

        public ModelResult<Page> AddPage(PageAddress parentPageAddress, PageInput pageInput, ServiceContext context)
        {
            pageInput = pluginEngine.Process<PluginPageInput>("ProcessInputOfPage", new PluginPageInput(pageInput)).ToPageInput();
            pageInput = pluginEngine.Process<PluginPageInput>("ProcessInputOfPageOnAdd", new PluginPageInput(pageInput)).ToPageInput();

            ValidationStateDictionary validationState = ValidatePageInput(pageInput);

            if (!validationState.IsValid) return new ModelResult<Page>(validationState);

            Guid siteID = getSiteID();
            Page page;

            using (TransactionScope transaction = new TransactionScope())
            {
                page = pageInput.ToPage(context.CurrentUser, siteID, EntityState.Normal);

                validatePage(page, validationState);

                if (!validationState.IsValid) return new ModelResult<Page>(validationState);

                page = repository.Save(page);

                invalidateCachedPageDependencies(page);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("PageSaved", new { context, page = new PageReadOnly(page) });
            pluginEngine.ExecuteAll("PageAdded", new { context, page = new PageReadOnly(page) });

            return new ModelResult<Page>(page, validationState);
        }

        public ModelResult<Page> EditPage(PageAddress pageAddress, PageInput pageInput, ServiceContext context)
        {
            pageInput = pluginEngine.Process<PluginPageInput>("ProcessInputOfPage", new PluginPageInput(pageInput)).ToPageInput();
            pageInput = pluginEngine.Process<PluginPageInput>("ProcessInputOfPageOnEdit", new PluginPageInput(pageInput)).ToPageInput();

            ValidationStateDictionary validationState = ValidatePageInput(pageInput);

            if (!validationState.IsValid) return new ModelResult<Page>(validationState);

            Page originalPage;
            Page newPage;
            bool isPublished;

            using (TransactionScope transaction = new TransactionScope())
            {
                originalPage = GetPage(pageAddress, context);
                isPublished = originalPage.State == EntityState.Normal && originalPage.Published.HasValue && originalPage.Published.Value <= DateTime.UtcNow;
                newPage = originalPage.Apply(pageInput, context.CurrentUser, EntityState.Normal);

                validatePage(newPage, originalPage, validationState);

                if (!validationState.IsValid) return new ModelResult<Page>(validationState);

                newPage = repository.Save(newPage);

                invalidateCachedPageForEdit(newPage, originalPage);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("PageSaved", new { context, page = new PageReadOnly(newPage) });
            pluginEngine.ExecuteAll("PageEdited", new { context, page = new PageReadOnly(newPage), pageOriginal = new PageReadOnly(originalPage) });

            bool isNowPublished = newPage.State == EntityState.Normal && newPage.Published.HasValue && newPage.Published.Value <= DateTime.UtcNow;
            if (!isPublished && isNowPublished)
                pluginEngine.ExecuteAll("PagePublished", new { context, page = new PageReadOnly(newPage) });
            else if (isPublished && !isNowPublished)
                pluginEngine.ExecuteAll("PageUnpublished", new { context, page = new PageReadOnly(newPage) });

            return new ModelResult<Page>(newPage, validationState);
        }

        public void RemovePage(PageAddress pageAddress, ServiceContext context)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                Page page = GetPage(pageAddress, context);

                if (page != null)
                {
                    if (repository.Remove(page.ID))
                    {
                        invalidateCachedPageForRemove(page);

                        transaction.Complete();

                        pluginEngine.ExecuteAll("PageRemoved", new { context, page = new PageReadOnly(page) });

                        return;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private Guid getSiteID()
        {
            return siteService.GetSite().ID;
        }

        private void invalidateCachedPageDependencies(Page page)
        {
            Page parent = page.Parent;

            while (parent != null)
            {
                cache.InvalidateItem(parent);

                parent = parent.Parent;
            }

            cache.InvalidateItem(page.Site);
        }

        private void invalidateCachedPageForEdit(Page newPage, Page originalPage)
        {
            if ((originalPage.Parent != null ? originalPage.Parent.ID : Guid.Empty) != (newPage.Parent != null ? newPage.Parent.ID : Guid.Empty))
            {
                if (originalPage.Parent != null)
                    invalidateCachedPageDependencies(originalPage.Parent);
                if (newPage.Parent != null)
                    invalidateCachedPageDependencies(newPage.Parent);
            }

            cache.InvalidateItem(newPage);
        }

        private void invalidateCachedPageForRemove(Page page)
        {
            invalidateCachedPageDependencies(page);

            cache.InvalidateItem(page);
        }

        private Page processDisplayOfPage(Func<Page> getPage, ServiceContext serviceContext)
        {
            Page page = getPage();
            PageForProcessing pageProxy = new PageForProcessing(page);

            pluginEngine.ExecuteAll("ProcessDisplayOfPage", new { context = serviceContext, page = pageProxy });

            return pageProxy.ToPage();
        }

        private void validatePage(Page newPage, ValidationStateDictionary validationState)
        {
            validatePage(newPage, newPage, validationState);
        }

        private void validatePage(Page newPage, Page originalPage, ValidationStateDictionary validationState)
        {
            ValidationState state = new ValidationState();
            Page foundPage;

            validationState.Add(typeof(Page), state);

            foundPage = repository.GetPage(getSiteID(), newPage.Slug, newPage.Parent.ID);

            if (foundPage != null && (newPage.Slug != originalPage.Slug || (originalPage.Parent != null ? originalPage.Parent.ID : Guid.Empty) != (newPage.Parent != null ? newPage.Parent.ID : Guid.Empty)))
                state.Errors.Add("Page.SlugNotUnique", newPage.Slug, "A page already exists with the same name under the same parent page");
        }

        #endregion
    }
}
