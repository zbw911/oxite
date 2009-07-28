// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System.Reflection;
using System.Text;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public static class SpamControlPluginExtensions
    {
        public static string ToQueryString(this object obj)
        {
            StringBuilder queryStringBuilder = new StringBuilder();

            foreach (PropertyInfo propertyInfo in (obj.GetType()).GetProperties())
                queryStringBuilder.AppendFormat("{0}={1}&", propertyInfo.Name, propertyInfo.GetValue(obj, null) ?? "");

            return queryStringBuilder.Remove(queryStringBuilder.Length - 1, 1).ToString();
        }
    }
}