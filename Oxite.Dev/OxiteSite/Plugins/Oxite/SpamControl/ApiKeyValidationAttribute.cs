// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using Oxite.Plugins.Attributes;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public class ApiKeyValidationAttribute : DefinitionAttribute
    {
        public ApiKeyValidationAttribute() : base(new ApiKeyValidator()) { }
    }
}
