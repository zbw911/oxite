﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;

namespace Oxite.Modules.FormsAuthentication.Models
{
    public class UserChangePasswordInput
    {
        public UserChangePasswordInput(string password, string passwordConfirm)
        {
            Password = password;
            PasswordConfirm = passwordConfirm;
        }

        public string Password { get; private set; }
        public string PasswordConfirm { get; private set; }
    }
}
