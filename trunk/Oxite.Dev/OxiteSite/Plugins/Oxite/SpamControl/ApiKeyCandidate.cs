// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public class ApiKeyCandidate
    {
        public Uri blog { get; set; }
        public string key { get; set; }
    }
}
