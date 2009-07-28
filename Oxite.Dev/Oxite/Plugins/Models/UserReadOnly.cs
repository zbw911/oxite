﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;

namespace Oxite.Plugins.Models
{
    public class UserReadOnly
    {
        public UserReadOnly(Oxite.Models.UserAuthenticated user)
        {
            Name = user.Name;
            DisplayName = user.DisplayName;
            Email = user.Email;
            EmailHash = user.EmailHash;
            Status = (State)(byte)user.Status;
            if (user.LanguageDefault != null)
                Language = user.LanguageDefault.Name;
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Email { get; private set; }
        public string EmailHash { get; private set; }
        public State Status { get; private set; }
        public string Language { get; private set; }
    }
}
