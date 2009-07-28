//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Linq;
using Oxite.Extensions;

namespace Oxite.Models.Extensions
{
    public static class PostExtensions
    {
        public static string GetBodyShort(this Post post)
        {
            return !string.IsNullOrEmpty(post.BodyShort) ? post.BodyShort : post.GetBodyShort(100);
        }

        //info: (nheskew) leaving in some tags will throw off the word count a little, but not enough to make it worth making perfect - yet
        public static string GetBodyShort(this Post post, int wordCount)
        {
            string bodyText = !string.IsNullOrEmpty(post.Body)
                ? post.Body.CleanHtmlTags("(/?p|br\\s*/|/?a\\s*[^<>]*?)").CleanWhitespace()
                : "";
            string previewText = "";

            if (!string.IsNullOrEmpty(bodyText))
                previewText = string.Join(" ", bodyText.Split(' ').Take(wordCount).ToArray()) +
                    (previewText != bodyText ? "&#160;&#8230;" : "");

            return previewText;
        }
    }
}
