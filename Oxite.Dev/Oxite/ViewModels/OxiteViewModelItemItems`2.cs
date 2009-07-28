//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;

namespace Oxite.ViewModels
{
    public class OxiteViewModelItemItems<T, K> : OxiteViewModelItem<T>
    {
        public OxiteViewModelItemItems()
        {
        }

        public OxiteViewModelItemItems(OxiteViewModel model)
        {
            Site = model.Site;
            User = model.User;
        }

        public IEnumerable<K> Items { get; set; }
    }
}
