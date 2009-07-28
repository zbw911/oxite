//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;

namespace Oxite.Plugins.Models
{
    public class CommentSmallReadOnly
    {
        public CommentSmallReadOnly(Oxite.Models.CommentSmall commentSmall)
        {
            Created = commentSmall.Created;
            CreatorName = commentSmall.CreatorName;
            CreatorEmailHash = commentSmall.CreatorEmailHash;
            CreatorUrl = commentSmall.CreatorUrl;
            Slug = commentSmall.Slug;
        }

        public DateTime Created { get; private set; }
        public string CreatorName { get; private set; }
        public string CreatorEmailHash { get; private set; }
        public string CreatorUrl { get; private set; }
        public string Slug { get; private set; }
    }
}
