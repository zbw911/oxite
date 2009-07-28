//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Blogs.Services
{
    public interface IPostService
    {
        IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, ServiceContext context);
        IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, TagAddress tagAddress, ServiceContext context);
        IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, BlogAddress blogAddress, ServiceContext context);
        IPageOfItems<Post> GetPosts(int pageIndex, int pageSize, ArchiveData archive, ServiceContext context);
        IPageOfItems<Post> GetPostsWithDrafts(int pageIndex, int pageSize, ServiceContext context);
        IPageOfItems<Post> GetPostsWithDrafts(int pageIndex, int pageSize, BlogAddress blogAddress, ServiceContext context);
        IEnumerable<Post> GetPosts(DateRangeAddress dateRangeAddress, ServiceContext context);
        IPageOfItems<Post> GetPostsByFileType(int pageIndex, int pageSize, string typeName, ServiceContext context);
        IEnumerable<DateTime> GetPostDateGroups(ServiceContext context);
        Post GetPost(PostAddress postAddress, ServiceContext context);
        Post GetRandomPost();
        ValidationStateDictionary ValidatePostInput(PostInput postInput);
        ModelResult<Post> AddPost(PostInput postInput, EntityState state, ServiceContext context);
        ModelResult<Post> AddPost(Blog blog, ImportPostInput postInput, ServiceContext context);
        ModelResult<Post> EditPost(PostAddress postAddress, PostInput postInput, EntityState state, ServiceContext context);
        void RemovePost(PostAddress postAddress, ServiceContext context);
        void RemoveAll(BlogAddress blogAddress);

        IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives();
        IEnumerable<KeyValuePair<ArchiveData, int>> GetArchives(BlogAddress blogAddress);

        ValidationStateDictionary AddTrackback(Post post, Trackback trackback);
        ValidationStateDictionary EditTrackback(Trackback trackback);
    }
}
