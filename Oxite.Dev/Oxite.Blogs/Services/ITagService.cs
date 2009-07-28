//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;

namespace Oxite.Modules.Blogs.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTags();
        IEnumerable<KeyValuePair<Tag, int>> GetTagsWithPostCount();
        Tag GetTag(TagAddress tagAddress);
        IEnumerable<KeyValuePair<Tag, int>> GetTagsUsedIn(BlogAddress blogAddress);
    }
}
