//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Models;
using Oxite.Repositories;

namespace Oxite.Modules.CMS.Repositories.SqlServer
{
    public class SqlServerPageRepository : IPageRepository
    {
        private readonly OxiteCMSDataContext context;

        public SqlServerPageRepository(OxiteCMSDataContext context)
        {
            this.context = context;
        }

        #region IPageRepository Members

        public Page GetPage(Guid siteID, string pagePartName, Guid parentPageID)
        {
            IQueryable<oxite_Page> query =
                from p in context.oxite_Pages
                where p.SiteID == siteID && p.Slug == pagePartName && ((parentPageID == Guid.Empty && p.ParentPageID == p.PageID) || p.ParentPageID == parentPageID)
                select p;

            return projectPages(query).FirstOrDefault();
        }

        public IQueryable<Page> GetPages(Guid siteID)
        {
            IQueryable<oxite_Page> query =
                from p in context.oxite_Pages
                where p.SiteID == siteID
                select p;

            return projectPages(query);
        }

        public Page Save(Page page)
        {
            oxite_Page pageToSave = null;

            if (page.ID != Guid.Empty && page.Site != null && page.Site.ID != Guid.Empty)
                pageToSave = context.oxite_Pages.FirstOrDefault(p => p.SiteID == page.Site.ID && p.PageID == page.ID);

            if (pageToSave == null)
            {
                pageToSave = new oxite_Page();

                pageToSave.SiteID = page.Site.ID;
                pageToSave.PageID = page.ID != Guid.Empty ? page.ID : Guid.NewGuid();
                pageToSave.CreatedDate = pageToSave.ModifiedDate = DateTime.UtcNow;

                context.oxite_Pages.InsertOnSubmit(pageToSave);
            }
            else
                pageToSave.ModifiedDate = DateTime.UtcNow;

            if (page.Parent != null && page.Parent.ID != Guid.Empty)
                pageToSave.ParentPageID = page.Parent.ID;
            else
                pageToSave.ParentPageID = pageToSave.PageID;
            
            pageToSave.Body = page.Body;
            pageToSave.PublishedDate = page.Published;
            pageToSave.Slug = page.Slug;
            pageToSave.State = (byte)page.State;
            pageToSave.Title = page.Title;

            oxite_User user = context.oxite_Users.FirstOrDefault(u => u.Username == page.Creator.Name);
            if (user == null) throw new InvalidOperationException(string.Format("User {0} could not be found", page.Creator.Name));
            pageToSave.CreatorUserID = user.UserID;

            context.SubmitChanges();

            return GetPage(pageToSave.SiteID, pageToSave.Slug, pageToSave.ParentPageID);
        }

        public bool Remove(Guid pageID)
        {
            oxite_Page foundPage = context.oxite_Pages.FirstOrDefault(p => p.PageID == pageID);
            bool removedPage = false;

            if (foundPage != null)
            {
                foundPage.State = (byte)EntityState.Removed;

                context.SubmitChanges();

                removedPage = true;
            }

            return removedPage;
        }

        #endregion

        #region Private Methods

        private IQueryable<Page> projectPages(IQueryable<oxite_Page> pages)
        {
            return
                from p in pages
                join u in context.oxite_Users on p.CreatorUserID equals u.UserID
                where p.State != (byte)EntityState.Removed
                orderby p.PublishedDate descending
                select new Page(p.SiteID, p.PageID, getParentPage(p), getHasChildren(p), new UserAuthenticated(u.UserID, u.Username, u.DisplayName, u.Email, u.HashedEmail, new Language(u.oxite_Language.LanguageID) { Name = u.oxite_Language.LanguageName, DisplayName = u.oxite_Language.LanguageDisplayName }, (EntityState)u.Status), p.Title, p.Body, p.Slug, (EntityState)p.State, p.CreatedDate, p.ModifiedDate, p.PublishedDate);
        }

        private Page getParentPage(oxite_Page page)
        {
            if (page.ParentPageID == page.PageID) return null;

            return (
                from p in context.oxite_Pages
                where p.PageID == page.ParentPageID
                select new Page(p.SiteID, p.PageID, getParentPage(p), p.Slug)
                ).FirstOrDefault();
        }

        private bool getHasChildren(oxite_Page page)
        {
            return (from p in context.oxite_Pages where p.ParentPageID == page.PageID select p).Any();
        }

        #endregion
    }
}
