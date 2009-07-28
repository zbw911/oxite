//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------

namespace Oxite.Modules.Setup.ModelBinders
{
    using System;
    using System.Collections.Specialized;
    using System.Web.Mvc;
    using Oxite.Modules.Setup.Models;

    /// <summary>
    /// Class to enable model binding for the SetupInput object.
    /// </summary>
    public class SetupInputModelBinder : IModelBinder
    {
        /// <summary>
        /// Method called when form data needs to be bound to a SetupInput instance.
        /// </summary>
        /// <param name="controllerContext">ControllerContext containing the form data to be bound.</param>
        /// <param name="bindingContext">ModelBindingContext</param>
        /// <returns>SetupInput instance bound to the data in the ControllerContext.</returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            NameValueCollection form = controllerContext.HttpContext.Request.Form;

            Guid siteId = Guid.Empty;

            string contextSiteId = controllerContext.RouteData.Values["siteId"].ToString();

            if (form.Get("siteId") != null)
            {
                siteId = new Guid(form.Get("siteId"));
            }
            else if (!string.IsNullOrEmpty(contextSiteId))
            {
                siteId = new Guid(contextSiteId);
            }

            string blogName = form.Get("blogName");
            string blogDisplayName = form.Get("blogDisplayName");
            string description = form.Get("blogDescription");

            string userName = form["userName"];
            string userDisplayName = form["userDisplayName"];
            string email = form["userEmail"];
            string password = form["userPassword"];
            string passwordConfirm = form["userPasswordConfirm"];
            string siteType = form["siteType"];

            bool commentingEnabled;
            bool.TryParse(form["blogCommentingEnabled"] != null ? form.GetValues("blogCommentingEnabled")[0] : "false", out commentingEnabled);

            SetupInput input = new SetupInput(siteId, blogName, blogDisplayName, description, !commentingEnabled, userName, userDisplayName, email, password, passwordConfirm);

            if (!string.IsNullOrEmpty(siteType))
            {
                input.SiteType = (SiteType)Enum.Parse(typeof(SiteType), siteType);
            }

            return input;
        }
    }
}
