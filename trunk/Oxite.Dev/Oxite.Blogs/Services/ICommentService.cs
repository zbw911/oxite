//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Blogs.Services
{
    public interface ICommentService
    {
        IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, bool includePending, bool sortDescending, ServiceContext context);
        IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Blog blog, ServiceContext context);
        IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Post post, bool includeUnapproved, ServiceContext context);
        IPageOfItems<Comment> GetComments(int pageIndex, int pageSize, Tag tag, ServiceContext context);
        ValidationStateDictionary ValidateCommentInput(CommentInput commentInput);
        ModelResult<Comment> AddComment(PostAddress postAddress, CommentInput commentInput, ServiceContext context);
        ModelResult<Comment> AddComment(Post post, ImportCommentInput commentInput, ServiceContext context);
        ModelResult<Comment> EditComment(CommentAddress commentAddress, CommentInput commentInput, ServiceContext context);
        bool RemoveComment(CommentAddress commentAddress, ServiceContext context);
        bool ApproveComment(CommentAddress commentAddress, ServiceContext context);
    }
}
