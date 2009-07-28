//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Models
{
    public class CommentAddress : PostAddress
    {
        public CommentAddress(string blogName, string postSlug, string commentSlug)
            : base(blogName, postSlug)
        {
            CommentSlug = commentSlug;
        }

        public CommentAddress(PostAddress postAddress, string commentSlug)
            : this(postAddress.BlogName, postAddress.PostSlug, commentSlug)
        {
        }

        public string CommentSlug { get; private set; }

        public PostAddress ToPostAddress()
        {
            return new PostAddress(BlogName, PostSlug);
        }
    }
}
