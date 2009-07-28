//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.Membership.Models;
using Oxite.Modules.Membership.Services;
using Oxite.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Membership.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IRoleService roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItems<UserAuthenticated> Find()
        {
            //TODO: (erikpo) Check permissions

            return new OxiteViewModelItems<UserAuthenticated>();
        }

        [ActionName("Find"), AcceptVerbs(HttpVerbs.Post)]
        public OxiteViewModelItems<UserAuthenticated> FindQuery(UserSearchCriteria searchCriteria)
        {
            //TODO: (erikpo) Check permissions

            IEnumerable<UserAuthenticated> foundUsers = userService.FindUsers(searchCriteria);
            //TODO: (erikpo) Before the list is set into the model, filter it from the AuthorizationManager class (like make sure anonymous doesn't ever get sent down, etc)
            OxiteViewModelItems<UserAuthenticated> model = new OxiteViewModelItems<UserAuthenticated> { Items = foundUsers };

            model.AddModelItem(searchCriteria);

            return model;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<UserAuthenticated> ItemAdd(UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            return new OxiteViewModelItem<UserAuthenticated>();
        }

        [ActionName("ItemAdd"), AcceptVerbs(HttpVerbs.Post)]
        public object ItemSaveAdd(UserInputAdd userInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<UserAuthenticated> results = userService.AddUser(userInput, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return ItemAdd(currentUser);
            }

            return Redirect(Url.AppPath(Url.ManageUsers()));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<UserAuthenticated> ItemEdit(UserAddress userAddress)
        {
            //TODO: (erikpo) Check permissions

            UserAuthenticated user = userService.GetUser(userAddress.UserName);

            if (user == null) return null;

            return new OxiteViewModelItem<UserAuthenticated> { Item = user };
        }

        [ActionName("ItemEdit"), AcceptVerbs(HttpVerbs.Post)]
        public object ItemSaveEdit(UserAddress userAddress, UserInputEdit userInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<UserAuthenticated> results = userService.EditUser(userAddress, userInput, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return ItemEdit(userAddress);
            }

            return Redirect(Url.AppPath(Url.ManageUsers()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object ItemRemove(UserAddress userAddress, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            UserAuthenticated user = userService.GetUser(userAddress.UserName);

            if (user == null) return null;

            userService.RemoveUser(userAddress, new ServiceContext(ControllerContext, currentUser));

            return Redirect(Url.AppPath(Url.ManageUsers()));
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItemItems<UserAuthenticated, Role> Roles(UserAddress userAddress)
        {
            //TODO: (erikpo) Check permissions

            UserAuthenticated user = userService.GetUser(userAddress.UserName);

            if (user == null) return null;

            IEnumerable<Role> roles = roleService.GetSiteRoles();

            return new OxiteViewModelItemItems<UserAuthenticated, Role> { Item = user, Items = roles };
        }
    }
}
