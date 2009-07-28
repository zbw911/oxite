//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Blogs.Extensions;

namespace Oxite.Modules.Blogs.Visitors
{
    public class CommentVisitor : Visitor
    {
        private readonly UrlHelper urlHelper;

        public CommentVisitor(UrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }

        public string Visit(HomePageContainer container, string dataFormat)
        {
            return urlHelper.Posts();
        }

        public string Visit(Blog blog, string dataFormat)
        {
            return urlHelper.Comments(blog, dataFormat);
        }

        public string Visit(Tag tag, string dataFormat)
        {
            return urlHelper.Comments(tag, dataFormat);
        }
    }
}
