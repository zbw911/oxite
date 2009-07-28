// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System.Web.Mvc;
using Oxite.Models;

namespace Oxite.Modules.BlogML.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string BlogML(this UrlHelper urlHelper, Blog blog)
        {
            return urlHelper.RouteUrl("BlogML", new { blogName = blog.Name });
        }
    }
}
