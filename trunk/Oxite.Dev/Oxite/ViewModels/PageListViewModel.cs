//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;

namespace Oxite.ViewModels
{
    public class PageListViewModel
    {
        public PageListViewModel(IEnumerable<Page> pages)
        {
            Pages = pages;
        }

        public IEnumerable<Page> Pages { get; private set; }
    }
}
