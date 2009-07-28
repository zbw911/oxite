//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Web;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Modules.CMS.Models;

namespace Oxite.Modules.CMS.ModelBinders
{
    public class PageInputModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request;

            DateTime? published = null;
            if (request.Form.IsTrue("isPublished"))
            {
                DateTime publishedValue;

                published = DateTime.TryParse(request.Form.Get("published"), out publishedValue) ? publishedValue : (DateTime?)null;
            }

            Guid parentID = Guid.Empty;
            if (!string.IsNullOrEmpty(request.Form.Get("parentID")))
                request.Form.Get("parentID").GuidTryParse(out parentID);

            string title = request.Form.Get("title");
            string body = request.Form.Get("body");
            string slug = request.Form.Get("slug");

            return new PageInput(parentID, title, body, slug, published);
        }
    }
}
