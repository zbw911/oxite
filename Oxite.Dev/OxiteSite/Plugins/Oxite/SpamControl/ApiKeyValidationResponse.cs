// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System.IO;
using System.Net;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public class ApiKeyValidationResponse
    {
        public ApiKeyValidationResponse(HttpWebResponse validationResponse)
        {
            IsValid = "valid" == new StreamReader(validationResponse.GetResponseStream()).ReadToEnd();
            Status = validationResponse.StatusCode;
            Message = validationResponse.GetResponseHeader("X-akismet-debug-help");
        }

        public readonly bool IsValid;
        public readonly HttpStatusCode Status;
        public readonly string Message;
    }
}
