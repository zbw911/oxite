﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;

namespace Oxite.Services
{
    public interface ILanguageService
    {
        Language GetLanguage(string name);
        void AddLanguage(Language language);
    }
}
