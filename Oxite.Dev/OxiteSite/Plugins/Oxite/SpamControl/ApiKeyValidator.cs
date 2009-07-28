// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Oxite.Plugins.Validators;
using Oxite.Validation;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public class ApiKeyValidator : IPluginValidator
    {
        public IEnumerable<ValidationError> Validate(IDictionary<string, KeyValuePair<Type, object>> pluginProperties)
        {
            SpamControlPlugin spamControlPlugin = new SpamControlPlugin {
                ApiEndpoint = pluginProperties.ContainsKey("ApiEndpoint") ? pluginProperties["ApiEndpoint"].Value as string : null,
                ApiKey = pluginProperties.ContainsKey("ApiKey") ? pluginProperties["ApiKey"].Value as string : null
            };

            ApiKeyValidationResponse apiKeyValidationResponse = runApiKeyValidation(spamControlPlugin.GetApiKeyVerificationUri("verify-key"), new ApiKeyCandidate { blog = null, key = spamControlPlugin.ApiKey });

            List<ValidationError> errors = new List<ValidationError>();
            if (apiKeyValidationResponse == null)
            {
                //todo: (nheskew) add more debug info to the error message
                errors.Add(new ValidationError("ApiKey", spamControlPlugin, "Plugins.Errors.ApiKeyValidator.UnableToValidate", "en", "The API key validation completely failed."));
                return errors;
            }

            if (!apiKeyValidationResponse.IsValid)
                errors.Add(new ValidationError("ApiKey", spamControlPlugin, "Plugins.Errors.ApiKeyValidator.InvalidKey", "en", "API key valididation failed. Message: {1}", apiKeyValidationResponse.Status, apiKeyValidationResponse.Message));
            
            return errors;
        }

        private ApiKeyValidationResponse runApiKeyValidation(Uri keyVerificationUri, ApiKeyCandidate apiKeyCandidate)
        {
            try
            {
                HttpWebRequest validationRequest = formRequest(keyVerificationUri, SpamControlPlugin.Version, apiKeyCandidate.ToQueryString());
                return new ApiKeyValidationResponse(validationRequest.GetResponse() as HttpWebResponse);
            }
            catch
            {
                return null;
            }
        }

        //todo: (nheskew) make use of GetPostResponse off context - somehow
        private static HttpWebRequest formRequest(Uri uri, string userAgent, string payload)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            webRequest.Method = "POST";
            webRequest.UserAgent = userAgent;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] payloadBytes = new ASCIIEncoding().GetBytes(payload);
            webRequest.ContentLength = payloadBytes.Length;

            StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream());
            streamWriter.Write(payload);
            streamWriter.Flush();
            streamWriter.Close();

            return webRequest;
        }
    }
}