//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;

namespace Oxite.Plugins.Models
{
    public class BlogReadOnly
    {
        public BlogReadOnly(Oxite.Models.Blog blog)
        {
            Name = blog.Name;
            DisplayName = blog.DisplayName;
            Description = blog.Description;
            CommentingDisabled = blog.CommentingDisabled;
            Created = blog.Created;
            Modified = blog.Modified;
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }
        public bool CommentingDisabled { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Modified { get; private set; }
    }
}
