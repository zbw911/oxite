//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;

namespace Oxite.ViewModels
{
    public class AdminDataViewModel
    {
        public AdminDataViewModel(IEnumerable<Post> posts, IEnumerable<Comment> comments)
        {
            Posts = posts;
            Comments = comments;
        }

        public IEnumerable<Post> Posts { get; private set; }
        public IEnumerable<Comment> Comments { get; private set; }
    }
}
