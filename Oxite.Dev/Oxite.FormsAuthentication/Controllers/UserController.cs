﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.FormsAuthentication.Extensions;
using Oxite.Modules.FormsAuthentication.Models;
using Oxite.Modules.FormsAuthentication.Services;
using Oxite.Modules.Membership.Models;
using Oxite.Modules.Membership.Services;
using Oxite.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.FormsAuthentication.Controllers
{
    public class UserController : Controller
    {
        private readonly IFormsAuthentication formsAuthentication;
        private readonly IUserService userService;
        private readonly IFormsAuthenticationUserService faUserService;

        public UserController(IFormsAuthentication formsAuthentication, IUserService userService, IFormsAuthenticationUserService faUserService)
        {
            this.formsAuthentication = formsAuthentication;
            this.userService = userService;
            this.faUserService = faUserService;
        }

        public OxiteViewModel SignIn()
        {
            return new OxiteViewModel { Container = new SignInPageContainer() };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object SignIn(string username, string password, bool rememberMe, string returnUrl)
        {
            //TODO: (erikpo) Move the following validation logic into a validator

            if (string.IsNullOrEmpty(username))
                ModelState.AddModelError("username", "You must specify a username.");

            if (string.IsNullOrEmpty(password))
                ModelState.AddModelError("password", "You must specify a password.");

            if (ViewData.ModelState.IsValid)
            {
                if (userService.SignIn(() => userService.GetUser(username, password), (u) => formsAuthentication.SetAuthCookie(u.Name, rememberMe), new ServiceContext(ControllerContext)))
                {
                    if (!string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith("/"))
                        return Redirect(returnUrl);

                    return Redirect(Url.AppPath(Url.Home()));
                }

                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return SignIn();
        }

        public ActionResult SignOut(UserAuthenticated currentUser)
        {
            formsAuthentication.SignOut();

            userService.SignOut(new ServiceContext(ControllerContext, currentUser));

            return Redirect(Url.AppPath(Url.Home()));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<UserAuthenticated> ChangePassword(UserAddress userAddress, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions or let the current user change their password

            UserAuthenticated user = userService.GetUser(userAddress.UserName);

            if (user == null) return null;

            return new OxiteViewModelItem<UserAuthenticated> { Item = user };
        }

        [ActionName("ChangePassword"), AcceptVerbs(HttpVerbs.Post)]
        public object ChangePasswordSave(UserAddress userAddress, UserChangePasswordInput userInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult results = faUserService.ChangePassword(userAddress, userInput, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return ChangePassword(userAddress, currentUser);
            }

            return Redirect(Url.AppPath(Url.ManageUsers()));
        }
    }
}
