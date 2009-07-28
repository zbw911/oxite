//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Oxite.Models;

namespace Oxite.Services
{
    public class ServiceContext : RequestContext
    {
        public ServiceContext(HttpContextBase httpContext, RouteData routeData)
            : base(httpContext, routeData)
        {
            if (routeData != null)
            {
                string dataFormat = routeData.Values["dataFormat"] as string;

                if (string.Compare(dataFormat, "RSS", true) == 0)
                    RequestDataFormat = RequestDataFormat.RSS;
                else if (string.Compare(dataFormat, "ATOM", true) == 0)
                    RequestDataFormat = RequestDataFormat.ATOM;
                else
                    RequestDataFormat = RequestDataFormat.Web;
            }
        }

        public ServiceContext(ControllerContext controllerContext)
            : this(controllerContext.HttpContext, controllerContext.RouteData)
        {
        }

        public ServiceContext(ControllerContext controllerContext, UserAuthenticated currentUser)
            : this(controllerContext.HttpContext, controllerContext.RouteData, currentUser)
        {
        }

        public ServiceContext(HttpContextBase httpContext, RouteData routeData, UserAuthenticated currentUser)
            : this(httpContext, routeData)
        {
            CurrentUser = currentUser;
        }

        public ServiceContext(ServiceContext context)
            : this(context.HttpContext, context.RouteData, context.CurrentUser)
        {
        }

        public UserAuthenticated CurrentUser { get; private set; }
        public RequestDataFormat RequestDataFormat { get; private set; }
        public DateTime? IfModifiedSince { get; private set; }
    }
}
