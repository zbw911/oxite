//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Modules.CMS.Models;

namespace Oxite.Modules.CMS.ModelBinders
{
    public class PageAddressModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return new PageAddress(controllerContext.RouteData.GetRequiredString("pagePath"));
        }
    }
}
