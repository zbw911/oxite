﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Infrastructure;

namespace Oxite.Models
{
    public class SiteSmall : ICacheItem
    {
        public SiteSmall(Guid id)
        {
            ID = id;
        }

        public Guid ID { get; private set; }

        #region ICacheItem Members

        public string GetCacheItemKey()
        {
            return string.Format("Site:{0:N}", ID);
        }

        public IEnumerable<ICacheItem> GetCacheDependencyItems()
        {
            return Enumerable.Empty<ICacheItem>();
        }

        #endregion
    }
}
