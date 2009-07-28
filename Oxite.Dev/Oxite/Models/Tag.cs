//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Infrastructure;
using System.Collections.Generic;

namespace Oxite.Models
{
    public class Tag : INamedEntity, ICacheItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime? Created { get; set; }

        #region INamedEntity Members

        string INamedEntity.Name { get { return Name; } }

        string INamedEntity.DisplayName { get { return DisplayName; } }

        #endregion

        #region ICacheItem Members

        public string GetCacheItemKey()
        {
            return ID.ToString("N");
        }

        public IEnumerable<ICacheItem> GetCacheDependencyItems()
        {
            return new List<ICacheItem>();
        }

        #endregion
    }
}
