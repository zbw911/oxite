﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Results;
using Oxite.ViewModels;

namespace Oxite.Filters
{
    public class ErrorExceptionFilter : IExceptionFilter
    {
        #region IExceptionFilter Members

        public void OnException(ExceptionContext filterContext)
        {
            ExceptionOxiteViewModel model = filterContext.Controller.ViewData.Model as ExceptionOxiteViewModel;

            model.Exception = filterContext.Exception;

            filterContext.HttpContext.AddError(filterContext.Exception);
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ErrorResult();
        }

        #endregion
    }
}
