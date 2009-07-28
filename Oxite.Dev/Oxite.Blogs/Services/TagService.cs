//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Repositories;

namespace Oxite.Modules.Blogs.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository repository;

        public TagService(ITagRepository repository)
        {
            this.repository = repository;
        }

        #region ITagService Members

        public IEnumerable<Tag> GetTags()
        {
            return repository.GetTags().ToArray();
        }

        public IEnumerable<KeyValuePair<Tag, int>> GetTagsWithPostCount()
        {
            return repository.GetTagsWithPostCount().ToArray();
        }

        public Tag GetTag(TagAddress tagAddress)
        {
            return repository.GetTag(tagAddress.TagName);
        }

        public IEnumerable<KeyValuePair<Tag, int>> GetTagsUsedIn(BlogAddress blogAddress)
        {
            return repository.GetTagsUsedInBlog(blogAddress.BlogName).ToArray();
        }

        #endregion
    }
}
