//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService tagService;

        public TagController(ITagService tagService)
        {
            this.tagService = tagService;
        }

        public OxiteViewModelItems<KeyValuePair<Tag, int>> Cloud()
        {
            return new OxiteViewModelItems<KeyValuePair<Tag, int>>
            {
                Container = new TagCloudPageContainer(),
                Items = tagService.GetTagsWithPostCount()
            };
        }
    }
}