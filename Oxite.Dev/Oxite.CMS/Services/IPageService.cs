//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;
using Oxite.Modules.CMS.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.CMS.Services
{
    public interface IPageService
    {
        Page GetPage(PageAddress pageAddress, ServiceContext context);
        IEnumerable<Page> GetPages();
        ValidationStateDictionary ValidatePageInput(PageInput pageInput);
        ModelResult<Page> AddPage(PageAddress parentPageAddress, PageInput pageInput, ServiceContext context);
        ModelResult<Page> EditPage(PageAddress pageAddress, PageInput pageInput, ServiceContext context);
        void RemovePage(PageAddress pageAddress, ServiceContext context);
    }
}
