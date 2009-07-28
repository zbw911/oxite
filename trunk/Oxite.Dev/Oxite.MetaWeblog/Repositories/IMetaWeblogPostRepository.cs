//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Models;

namespace Oxite.Modules.MetaWeblog.Repositories
{
    public interface IMetaWeblogPostRepository
    {
        Post GetPost(Guid id);
        bool Remove(Guid postID);
    }
}
