//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.CMS.Repositories
{
    public interface IPageRepository
    {
        Page GetPage(Guid siteID, string pagePartName, Guid parentPageID);
        IQueryable<Page> GetPages(Guid siteID);
        Page Save(Page page);
        bool Remove(Guid pageID);
    }
}
