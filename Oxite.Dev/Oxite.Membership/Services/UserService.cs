//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Oxite.Extensions;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Membership.Extensions;
using Oxite.Modules.Membership.Models;
using Oxite.Modules.Membership.Repositories;
using Oxite.Plugins.Extensions;
using Oxite.Plugins.Models;
using Oxite.Repositories;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Membership.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;
        private readonly ILanguageRepository languageRepository;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ISiteService siteService;
        private readonly IOxiteCacheModule cache;

        public UserService(IUserRepository repository, ILanguageRepository languageRepository, IValidationService validator, IPluginEngine pluginEngine, ISiteService siteService, IModulesLoaded modules)
        {
            this.repository = repository;
            this.languageRepository = languageRepository;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.siteService = siteService;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
        }

        #region IUserService Members

        public UserAuthenticated GetUser(string name)
        {
            return cache.GetItem<UserAuthenticated>(
                string.Format("GetUser-UserName:{0}", name),
                () => repository.GetUser(getSite().ID, name),
                null
                );
        }

        public UserAuthenticated GetUserByModuleData(string moduleName, string data)
        {
            return cache.GetItem<UserAuthenticated>(
                string.Format("GetUserByModuleData-ModuleName:{0},Data:{1}", moduleName, data),
                () => repository.GetUserByModuleData(getSite().ID, moduleName, data),
                null
                );
        }

        public IEnumerable<UserAuthenticated> FindUsers(UserSearchCriteria criteria)
        {
            return repository.FindUsers(criteria).ToArray();
        }

        public ModelResult<UserAuthenticated> AddUser(UserInputAdd userInputAdd, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(UserInputAdd), validator.Validate(userInputAdd));

            if (!validationState.IsValid) return new ModelResult<UserAuthenticated>(validationState);

            Site site = getSite();
            //TODO: (erikpo) Replace with some logic to set the language from the user's browser or from a dropdown list
            Language language = languageRepository.GetLanguage(site.LanguageDefault ?? "en");
            UserAuthenticated user;

            using (TransactionScope transaction = new TransactionScope())
            {
                user = userInputAdd.ToUser(e => e.ComputeHash(), EntityState.Normal, language);

                validateUser(user, validationState);

                if (!validationState.IsValid) return new ModelResult<UserAuthenticated>(validationState);

                user = repository.Save(user, site.ID);

                invalidateCachedUserDependencies(user);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("UserSaved", new { context, user = new UserReadOnly(user) });
            pluginEngine.ExecuteAll("UserAdded", new { context, user = new UserReadOnly(user) });

            return new ModelResult<UserAuthenticated>(user, validationState);
        }

        public ModelResult<UserAuthenticated> EditUser(UserAddress userAddress, UserInputEdit userInputEdit, ServiceContext context)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(UserInputEdit), validator.Validate(userInputEdit));

            if (!validationState.IsValid) return new ModelResult<UserAuthenticated>(validationState);

            Guid siteID = getSite().ID;
            UserAuthenticated originalUser;
            UserAuthenticated newUser;

            using (TransactionScope transaction = new TransactionScope())
            {
                originalUser = repository.GetUser(siteID, userAddress.UserName);
                newUser = originalUser.Apply(userInputEdit, e => e.ComputeHash(), EntityState.Normal);

                validateUser(newUser, originalUser, validationState);

                if (!validationState.IsValid) return new ModelResult<UserAuthenticated>(validationState);

                newUser = repository.Save(newUser, siteID);

                invalidateCachedUserForEdit(newUser, originalUser);

                transaction.Complete();
            }

            pluginEngine.ExecuteAll("UserSaved", new { context, user = new UserReadOnly(newUser) });
            pluginEngine.ExecuteAll("UserEdited", new { context, user = new UserReadOnly(newUser), userOriginal = new UserReadOnly(originalUser) });

            return new ModelResult<UserAuthenticated>(newUser, validationState);
        }

        public void RemoveUser(UserAddress userAddress, ServiceContext context)
        {
            Guid siteID = getSite().ID;

            using (TransactionScope transaction = new TransactionScope())
            {
                UserAuthenticated user = repository.GetUser(siteID, userAddress.UserName);

                if (user != null)
                {
                    if (repository.Remove(user.ID))
                    {
                        invalidateCachedUserForRemove(user);

                        transaction.Complete();

                        pluginEngine.ExecuteAll("UserRemoved", new { context, user = new UserReadOnly(user) });

                        return;
                    }
                }
            }
        }

        public string GetModuleData(Guid userID, string moduleName)
        {
            return repository.GetModuleData(getSite().ID, userID, moduleName);
        }

        public string GetModuleData(string userName, string moduleName)
        {
            return repository.GetModuleData(getSite().ID, userName, moduleName);
        }

        public void SetModuleData(string userName, string moduleName, string data)
        {
            repository.SetModuleData(getSite().ID, userName, moduleName, data);
        }

        public bool SignIn(string name, ServiceContext serviceContext)
        {
            return SignIn(() => GetUser(name), null, serviceContext);
        }

        public bool SignIn(Func<UserAuthenticated> getUser, Action<UserAuthenticated> afterSignIn, ServiceContext serviceContext)
        {
            UserAuthenticated user = getUser();

            if (user != null)
            {
                if (afterSignIn != null)
                    afterSignIn(user);

                pluginEngine.ExecuteAll("UserSignedIn", new { context = new ServiceContext(serviceContext.HttpContext, serviceContext.RouteData, user), user = new UserReadOnly(user) });

                return true;
            }

            return false;
        }

        public void SignOut(ServiceContext serviceContext)
        {
            pluginEngine.ExecuteAll("UserSignedOut", new { context = new ServiceContext(serviceContext.HttpContext, serviceContext.RouteData, null), user = new UserReadOnly(serviceContext.CurrentUser) });
        }

        public void EnsureAnonymousUser()
        {
            UserAuthenticated user = GetUser("Anonymous");

            if (user == null)
            {
                Site site = getSite();
                //TODO: (erikpo) Replace with some logic to set the language from the user's browser or from a dropdown list
                Language language = languageRepository.GetLanguage(site.LanguageDefault ?? "en");

                user = new UserAuthenticated("Anonymous", "", "", "", language, EntityState.Normal);

                repository.Save(user, getSite().ID);
            }
        }

        #endregion

        #region Private Methods

        private Site getSite()
        {
            return siteService.GetSite();
        }

        private void invalidateCachedUserDependencies(UserAuthenticated user)
        {
            // UserAuthenticated doesn't have any dependencies
        }

        private void invalidateCachedUserForEdit(UserAuthenticated newUser, UserAuthenticated originalUser)
        {
            // UserAuthenticated doesn't have any dependencies

            cache.InvalidateItem(newUser);
        }

        private void invalidateCachedUserForRemove(UserAuthenticated user)
        {
            invalidateCachedUserDependencies(user);

            cache.InvalidateItem(user);
        }

        private void validateUser(UserAuthenticated newUser, ValidationStateDictionary validationState)
        {
            validateUser(newUser, newUser, validationState);
        }

        private void validateUser(UserAuthenticated newUser, UserAuthenticated originalUser, ValidationStateDictionary validationState)
        {
            ValidationState state = new ValidationState();
            UserAuthenticated foundUser;

            validationState.Add(typeof(UserAuthenticated), state);

            foundUser = repository.GetUser(getSite().ID, newUser.Name);

            if (foundUser != null && newUser.Name != originalUser.Name)
                state.Errors.Add(new ValidationError("User.NameNotUnique", newUser.Name, "A user already exists with the supplied name"));
        }

        #endregion
    }
}
