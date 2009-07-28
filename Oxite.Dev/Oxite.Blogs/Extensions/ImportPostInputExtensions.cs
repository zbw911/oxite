//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Modules.Blogs.Models;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Extensions
{
    public static class ImportPostInputExtensions
    {
        public static ImportPostInput NormalizeTags(this ImportPostInput post, Func<string, string> normalizeTag)
        {
            if (post.Tags == null || post.Tags.Count() == 0) return post;

            List<Tag> tags = new List<Tag>(post.Tags.Count());

            post.Tags.ToList().ForEach(t => tags.Add(new Tag() { ID = t.ID, Created = t.Created, DisplayName = t.DisplayName, Name = normalizeTag(t.Name) }));

            return new ImportPostInput(post.Body, post.BodyShort, post.CommentingDisabled, post.Created, post.Creator, post.Modified, post.Published, post.Slug, post.State, tags, post.Title, post.Trackbacks);
        }
    }
}
