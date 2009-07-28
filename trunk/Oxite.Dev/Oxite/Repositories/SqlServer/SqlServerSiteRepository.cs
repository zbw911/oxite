﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;
using Oxite.Repositories;

namespace Oxite.Repositories.SqlServer
{
    public class SqlServerSiteRepository : ISiteRepository
    {
        private readonly OxiteDataContext context;

        public SqlServerSiteRepository(OxiteDataContext context)
        {
            this.context = context;
        }

        #region ISiteRepository Members

        public Site GetSite(string name)
        {
            var query =
                from s in context.oxite_Sites
                where s.SiteName == name
                select s;

            return projectSites(query).FirstOrDefault();
        }

        public void Save(Site site)
        {
            oxite_Site dbSite = null;

            if (site.ID != Guid.Empty)
            {
                dbSite = (from s in context.oxite_Sites where s.SiteID == site.ID || s.SiteName == site.Name select s).FirstOrDefault();
            }

            if (dbSite == null)
            {
                dbSite = new oxite_Site();

                context.oxite_Sites.InsertOnSubmit(dbSite);

                if (site.ID == Guid.Empty)
                {
                    dbSite.SiteID = Guid.NewGuid();
                }
                else
                {
                    dbSite.SiteID = site.ID;
                }

                makeFieldsMatch(dbSite, site);
            }
            else
            {
                makeFieldsMatch(dbSite, site);

                var siteRedirectsQuery =
                    from sr in context.oxite_SiteRedirects
                    where sr.SiteID == dbSite.SiteID
                    select sr;

                if (siteRedirectsQuery.Count() > 0)
                {
                    context.oxite_SiteRedirects.DeleteAllOnSubmit(siteRedirectsQuery);
                }
            }

            if (site.HostRedirects != null && site.HostRedirects.Count > 0)
            {
                context.oxite_SiteRedirects.InsertAllOnSubmit(
                    site.HostRedirects.Select(
                        hr =>
                            new oxite_SiteRedirect
                            {
                                SiteID = dbSite.SiteID,
                                SiteRedirect = hr.ToString()
                            }
                        )
                    );
            }

            context.SubmitChanges();
        }

        #endregion

        #region Private Methods

        private IQueryable<Site> projectSites(IQueryable<oxite_Site> query)
        {
            return
                from s in query
                let redirects = getRedirects(s)
                select new Site
                {
                    ID = s.SiteID,
                    Host = new Uri(s.SiteHost),
                    Name = s.SiteName,
                    DisplayName = s.SiteDisplayName,
                    Description = s.SiteDescription,
                    LanguageDefault = s.LanguageDefault,
                    TimeZoneOffset = s.TimeZoneOffset,
                    PageTitleSeparator = s.PageTitleSeparator,
                    FavIconUrl = s.FavIconUrl,
                    CommentStateDefault = s.CommentStateDefault,
                    IncludeOpenSearch = s.IncludeOpenSearch,
                    AuthorAutoSubscribe = s.AuthorAutoSubscribe,
                    PostEditTimeout = s.PostEditTimeout,
                    GravatarDefault = s.GravatarDefault,
                    SkinsPath = s.SkinsPath,
                    SkinsScriptsPath = s.SkinsScriptsPath,
                    SkinsStylesPath = s.SkinsStylesPath,
                    Skin = s.Skin,
                    AdminSkin = s.AdminSkin,
                    HasMultipleBlogs = s.HasMultipleBlogs,
                    RouteUrlPrefix = s.RouteUrlPrefix,
                    CommentingDisabled = s.CommentingDisabled,
                    PluginsPath = s.PluginsPath,
                    HostRedirects = projectSiteRedirects(redirects)
                };
        }

        private IQueryable<oxite_SiteRedirect> getRedirects(oxite_Site site)
        {
            return
                from sr in context.oxite_SiteRedirects
                where sr.SiteID == site.SiteID
                select sr;
        }

        private IList<Uri> projectSiteRedirects(IQueryable<oxite_SiteRedirect> siteRedirects)
        {
            var query =
                from sr in siteRedirects
                select new Uri(sr.SiteRedirect);

            return query.ToList();
        }

        private void makeFieldsMatch(oxite_Site dbSite, Site site)
        {
            dbSite.SiteHost = site.Host.ToString();
            dbSite.SiteName = site.Name;
            dbSite.SiteDisplayName = site.DisplayName;
            dbSite.SiteDescription = site.Description;
            dbSite.LanguageDefault = site.LanguageDefault;
            dbSite.TimeZoneOffset = site.TimeZoneOffset;
            dbSite.PageTitleSeparator = site.PageTitleSeparator;
            dbSite.FavIconUrl = site.FavIconUrl;
            dbSite.CommentStateDefault = site.CommentStateDefault;
            dbSite.IncludeOpenSearch = site.IncludeOpenSearch;
            dbSite.AuthorAutoSubscribe = site.AuthorAutoSubscribe;
            dbSite.PostEditTimeout = site.PostEditTimeout;
            dbSite.GravatarDefault = site.GravatarDefault;
            dbSite.SkinsPath = site.SkinsPath;
            dbSite.SkinsScriptsPath = site.SkinsScriptsPath;
            dbSite.SkinsStylesPath = site.SkinsStylesPath;
            dbSite.Skin = site.Skin;
            dbSite.AdminSkin = site.AdminSkin;
            dbSite.HasMultipleBlogs = site.HasMultipleBlogs;
            dbSite.RouteUrlPrefix = site.RouteUrlPrefix;
            dbSite.CommentingDisabled = site.CommentingDisabled;
            dbSite.PluginsPath = site.PluginsPath;
        }

        #endregion
    }
}
