//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Services;
using Oxite.ViewModels;
using Oxite.Modules.Blogs.Services;

namespace Oxite.Modules.Blogs.Controllers
{
    public class SiteMapController : Controller
    {
        private readonly IPostService postService;

        public SiteMapController(IPostService postService)
        {
            this.postService = postService;
        }

        public OxiteViewModelItems<DateTime> SiteMapIndex()
        {
            IEnumerable<DateTime> postDateGroups = postService.GetPostDateGroups(new ServiceContext(ControllerContext));

            return new OxiteViewModelItems<DateTime> { Items = postDateGroups };
        }

        public OxiteViewModelItems<Post> SiteMap(OneMonthDateRangeAddress dateRangeAddress)
        {
            return new OxiteViewModelItems<Post> { Items = postService.GetPosts(dateRangeAddress, new ServiceContext(ControllerContext)) };
        }
    }
}
