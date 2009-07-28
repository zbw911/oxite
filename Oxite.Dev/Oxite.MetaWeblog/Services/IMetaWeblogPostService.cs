//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;
using Oxite.Services;

namespace Oxite.Modules.MetaWeblog.Services
{
    public interface IMetaWeblogPostService
    {
        Post GetPost(MetaWeblogPostAddress postAddress, ServiceContext context);
        void RemovePost(MetaWeblogPostAddress postAddress, ServiceContext context);
    }
}
