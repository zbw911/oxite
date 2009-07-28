//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;
using Oxite.Modules.Search.Models;
using Oxite.Services;

namespace Oxite.Modules.Search.Services
{
    public interface ISearchPostService
    {
        IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, SearchCriteria criteria, ServiceContext context);
    }
}
