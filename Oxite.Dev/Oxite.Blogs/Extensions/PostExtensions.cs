//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Infrastructure;

namespace Oxite.Modules.Blogs.Extensions
{
    public static class PostExtensions
    {
        public static Post Apply(this Post post, PostInput input, EntityState state, UserAuthenticated creator)
        {
            List<Tag> tags = new List<Tag>(post.Tags.Count());

            foreach (string tagName in input.Tags)
                tags.Add(new Tag { Name = tagName });

            return new Post(post.Blog, input.Body, input.BodyShort, input.CommentingDisabled, post.Created, creator, post.ID, post.CommentCount, post.Modified, input.Published, input.Slug, state, tags, input.Title, post.Trackbacks);
        }

        public static PostAddress ToPostAddress(this Post post)
        {
            return new PostAddress(post.Blog.ToBlogAddress(), post.Slug);
        }

        public static IEnumerable<ICacheItem> GetPostDependencies(this Post post)
        {
            List<ICacheItem> dependencies = new List<ICacheItem>();

            dependencies.Add(post);

            dependencies.Add(post.Blog);

            dependencies.AddRange(post.Tags.Cast<ICacheItem>());

            return dependencies;
        }
    }
}
