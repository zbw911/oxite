//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using Oxite.Models;

namespace Oxite.Modules.CMS.Models
{
    public class PageAddress
    {
        public PageAddress(string pagePath)
        {
            PageMode = getPageMode(pagePath);
            PagesInPath = getPagesInpath(pagePath);
        }

        public IEnumerable<PagePartAddress> PagesInPath { get; private set; }
        public PageMode PageMode { get; private set; }

        private IEnumerable<PagePartAddress> getPagesInpath(string pagePath)
        {
            string pagePartsPath = PageMode == PageMode.NotSet ? pagePath : pagePath.Remove(pagePath.Length - (PageMode.ToString().Length + 1));
            string[] pagePartsValues = pagePartsPath.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            PagePartAddress[] pageParts = new PagePartAddress[pagePartsValues.Length];

            for (int i = 0; i < pagePartsValues.Length; i++)
                pageParts[i] = new PagePartAddress(pagePartsValues[i]);

            return pageParts;
        }

        private PageMode getPageMode(string pagePath)
        {
            PageMode mode = PageMode.NotSet;

            if (pagePath.EndsWith("/" + PageMode.Add, StringComparison.OrdinalIgnoreCase))
                mode = PageMode.Add;

            if (pagePath.EndsWith("/" + PageMode.Edit, StringComparison.OrdinalIgnoreCase))
                mode = PageMode.Edit;

            if (pagePath.EndsWith("/" + PageMode.Remove, StringComparison.OrdinalIgnoreCase))
                mode = PageMode.Remove;

            return mode;
        }
    }
}
