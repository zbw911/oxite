//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;
using Oxite.Modules.Blogs.Models;

namespace Oxite.Modules.Blogs.Extensions
{
    public static class CommentExtensions
    {
        public static Comment Apply(this Comment comment, CommentInput input, UserAuthenticated creator)
        {
            if (creator != null)
                return new Comment(input.Body, comment.Created, creator, comment.CreatorIP, comment.CreatorUserAgent, comment.ID, comment.Language, comment.Modified, new CommentSmall(input.ParentID), comment.Post, comment.Slug, comment.State);
            else
                return new Comment(input.Body, comment.Created, input.Creator, comment.CreatorIP, comment.CreatorUserAgent, comment.ID, comment.Language, comment.Modified, new CommentSmall(input.ParentID), comment.Post, comment.Slug, comment.State);
        }
    }
}
