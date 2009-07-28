//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Linq;
using Oxite.Extensions;

namespace Oxite.Models.Extensions
{
    public static class PageExtensions
    {
        public static string GetBodyShort(this Page page)
        {
            return page.GetBodyShort(100);
        }

        public static string GetBodyShort(this Page page, int wordCount)
        {
            string previewText = !string.IsNullOrEmpty(page.Body) ? page.Body.CleanHtmlTags().CleanWhitespace() : "";

            if (!string.IsNullOrEmpty(previewText))
                previewText = string.Join(" ", previewText.Split(' ').Take(wordCount).ToArray());

            return previewText;
        }
    }
}
