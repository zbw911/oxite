﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using Oxite.Models;
using Oxite.Modules.Membership.Models;
using Oxite.Services;

namespace Oxite.Modules.Membership.Services
{
    public interface IUserService
    {
        UserAuthenticated GetUser(string name);
        UserAuthenticated GetUserByModuleData(string moduleName, string data);
        IEnumerable<UserAuthenticated> FindUsers(UserSearchCriteria criteria);
        ModelResult<UserAuthenticated> AddUser(UserInputAdd userInput, ServiceContext context);
        ModelResult<UserAuthenticated> EditUser(UserAddress userAddress, UserInputEdit userInput, ServiceContext context);
        void RemoveUser(UserAddress userAddress, ServiceContext context);
        string GetModuleData(string userName, string moduleName);
        string GetModuleData(Guid userID, string moduleName);
        void SetModuleData(string userName, string moduleName, string data);
        void EnsureAnonymousUser();
        bool SignIn(string name, ServiceContext serviceContext);
        bool SignIn(Func<UserAuthenticated> getUser, Action<UserAuthenticated> afterSignIn, ServiceContext serviceContext);
        void SignOut(ServiceContext serviceContext);
    }
}
