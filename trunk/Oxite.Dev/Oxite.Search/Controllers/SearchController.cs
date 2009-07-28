//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Services;
using Oxite.ViewModels;
using Oxite.Models;
using Oxite.Modules.Search.Models;
using Oxite.Modules.Search.Services;

namespace Oxite.Modules.Search.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchPostService searchPostService;

        public SearchController(ISearchPostService searchPostService)
        {
            this.searchPostService = searchPostService;
        }

        public OxiteViewModelItems<Post> List(int? pageNumber, int pageSize, SearchCriteria criteria, UserAuthenticated currentUser)
        {
            int pageIndex = pageNumber.HasValue ? pageNumber.Value - 1 : 0;

            return new OxiteViewModelItems<Post>
            {
                Container = new SearchPageContainer(),
                Items = criteria.HasCriteria() ? searchPostService.GetPosts(pageIndex, pageSize, criteria, new ServiceContext(ControllerContext, currentUser)) : null
            };
        }
    }
}
