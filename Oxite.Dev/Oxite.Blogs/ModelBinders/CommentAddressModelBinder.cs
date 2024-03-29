﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;

namespace Oxite.Modules.Blogs.ModelBinders
{
    public class CommentAddressModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new CommentAddress(
                new PostAddress(
                    new BlogAddress(
                        controllerContext.RouteData.GetRequiredString("blogName")
                        ),
                    controllerContext.RouteData.GetRequiredString("postSlug")
                    ),
                controllerContext.RouteData.GetRequiredString("commentSlug")
                );
        }
    }
}
