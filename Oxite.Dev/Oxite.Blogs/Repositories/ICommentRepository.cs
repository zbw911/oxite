//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;

namespace Oxite.Modules.Blogs.Repositories
{
    public interface ICommentRepository
    {
        Comment GetComment(Guid commentID);
        Comment GetComment(Guid siteID, string blogName, string postSlug, string commentSlug);
        IQueryable<Comment> GetComments(Guid siteID, bool includePending, bool sortDescending);
        IQueryable<Comment> GetCommentsByBlog(Guid blogID);
        IQueryable<Comment> GetCommentsByPost(Guid postID, bool includeUnapproved);
        IQueryable<Comment> GetCommentsByTag(Guid siteID, Guid tagID);
        void ChangeState(Guid commentID, EntityState state);
        Comment Save(Comment comment, Guid siteID, string blogName, string postSlug);
    }
}
