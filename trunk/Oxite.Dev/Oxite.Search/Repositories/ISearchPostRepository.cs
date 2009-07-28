//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using Oxite.Models;
using Oxite.Modules.Search.Models;

namespace Oxite.Modules.Search.Repositories
{
    public interface ISearchPostRepository
    {
        IQueryable<Post> GetPosts(Guid siteID, SearchCriteria criteria);
    }
}
