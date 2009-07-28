//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;

namespace Oxite.Models.Extensions
{
    public static class IPageOfItemsExtensions
    {
        public static IPageOfItems<Post> Since(this IPageOfItems<Post> pageOfItems, DateTime? sinceDate)
        {
            if (sinceDate.HasValue)
                return new PageOfItems<Post>(pageOfItems.Where(p => p.Published.Value > sinceDate.Value), pageOfItems.PageIndex, pageOfItems.PageSize, pageOfItems.TotalItemCount);

            return pageOfItems;
        }
    }
}
