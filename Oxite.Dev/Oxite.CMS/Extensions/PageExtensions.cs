//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;
using Oxite.Modules.CMS.Models;

namespace Oxite.Modules.CMS.Extensions
{
    public static class PageExtensions
    {
        public static Page Apply(this Page page, PageInput input, UserAuthenticated creator, EntityState state)
        {
            return new Page(input.Body, creator, page.Site.ID, page.ID, input.ParentID, input.Published, input.Slug, state, input.Title);
        }
    }
}
