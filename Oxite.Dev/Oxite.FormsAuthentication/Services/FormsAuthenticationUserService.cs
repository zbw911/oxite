//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.FormsAuthentication.Extensions;
using Oxite.Modules.FormsAuthentication.Models;
using Oxite.Modules.Membership.Models;
using Oxite.Modules.Membership.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.FormsAuthentication.Services
{
    public class FormsAuthenticationUserService : IFormsAuthenticationUserService
    {
        private readonly IUserRepository repository;
        private readonly IValidationService validator;
        private readonly ISiteService siteService;
        private readonly IPluginEngine pluginEngine;

        public FormsAuthenticationUserService(IUserRepository repository, IValidationService validator, ISiteService siteService, IPluginEngine pluginEngine)
        {
            this.repository = repository;
            this.validator = validator;
            this.siteService = siteService;
            this.pluginEngine = pluginEngine;
        }

        #region IFormsAuthenticationUserService Members

        public ModelResult ChangePassword(UserAddress userAddress, UserChangePasswordInput userInput, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(UserChangePasswordInput), validator.Validate(userInput));

            if (!validationState.IsValid) return new ModelResult(validationState);

            UserAuthenticated user = repository.GetUser(getSiteID(), userAddress.UserName);
            string passwordSalt = Guid.NewGuid().ToString("N");
            string password = userInput.Password.SaltAndHash(passwordSalt);

            repository.SetModuleData(getSiteID(), user.ID, "FormsAuthentication", string.Format("{0}|{1}", passwordSalt, password));

            pluginEngine.ExecuteAll("UserChangePassword", new { user, context });

            return new ModelResult();
        }

        #endregion

        #region Private Methods

        public Guid getSiteID()
        {
            return siteService.GetSite().ID;
        }

        #endregion
    }
}
