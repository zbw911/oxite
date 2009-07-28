//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Models;

namespace Oxite.Modules.CMS.Models
{
    public class PageInput
    {
        public PageInput(Guid parentID, string title, string body, string slug, DateTime? published)
        {
            ParentID = parentID;
            Title = title;
            Body = body;
            Slug = slug;
            Published = published;
        }

        public PageInput(Page page, PageInput input)
        {
            ParentID = input.ParentID != Guid.Empty ? input.ParentID : page.Parent != null ? page.Parent.ID : page.ID;
            Title = !string.IsNullOrEmpty(input.Title) ? input.Title : page.Title;
            Body = !string.IsNullOrEmpty(input.Body) ? input.Body : page.Body;
            Slug = !string.IsNullOrEmpty(input.Slug) ? input.Slug : page.Slug;
            Published = input.Published.HasValue ? input.Published : page.Published;
        }

        public Guid ParentID { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public string Slug { get; private set; }
        public DateTime? Published { get; private set; }

        public Page ToPage(UserAuthenticated creator, Guid siteID, EntityState state)
        {
            return new Page(Body, creator, siteID, Guid.Empty, ParentID, Published, Slug, state, Title);
        }
    }
}
