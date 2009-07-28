//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
namespace Oxite.Modules.Setup.Services
{
    using System;
    using Oxite.Infrastructure;
    using Oxite.Models;
    using Oxite.Modules.Blogs.Models;
    using Oxite.Modules.Blogs.Services;
    using Oxite.Modules.Membership.Models;
    using Oxite.Modules.Membership.Repositories;
    using Oxite.Modules.Membership.Services;
    using Oxite.Services;
    using Oxite.Validation;
    using Oxite.Modules.Setup.Models;
    using System.Transactions;

    public class SetupService : ISetupService
    {
        #region Fields
        private readonly ISiteService siteService;
        private readonly IUserService userService;
        private readonly IUserRepository repository;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly ILanguageService languageService;
        private readonly IBlogService blogService;
        #endregion

        #region Constructor
        public SetupService(ISiteService siteService, IUserService userService, IUserRepository repository, IValidationService validator, IPluginEngine pluginEngine, ILanguageService languageService, IBlogService blogService)
        {
            this.siteService = siteService;
            this.userService = userService;
            this.repository = repository;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.languageService = languageService;
            this.blogService = blogService;
        }
        #endregion

        public Site SetupSite(SetupInput input, ServiceContext serviceContext, out ValidationStateDictionary validationState)
        {
            Site site = null;

            using (TransactionScope transaction = new TransactionScope())
            {
                site = this.CreateSite(out validationState);

                if (!validationState.IsValid)
                {
                    return null;
                }

                BlogInput blogInput = new BlogInput(input.BlogName, input.BlogDisplayName, input.Description, input.CommentingDisabled);

                ModelResult<Blog> blogResults = this.CreateBlog(blogInput, serviceContext);

                if (!blogResults.IsValid)
                {
                    validationState = blogResults.ValidationState;
                    return null;
                }

                UserInputAdd userInput = new UserInputAdd(input.UserName, input.AdminDisplayName, input.Email, input.Password, input.PasswordConfirm);

                ModelResult<UserAuthenticated> results = this.SetupUser(userInput, serviceContext);

                if (!results.IsValid)
                {
                    validationState = results.ValidationState;

                    return null;
                }

                transaction.Complete();
            }

            return site;
        }

        private ModelResult<Blog> CreateBlog(BlogInput blogInput, ServiceContext serviceContext)
        {
            return this.blogService.AddBlog(blogInput, serviceContext);
        }

        private Site CreateSite(out ValidationStateDictionary validationState)
        {
            Site site = this.siteService.GetSite();

            if (site != null)
            {
                //throw new InvalidOperationException("A site has already been set up.  Please contact the administrator to make changes.");
            }

            Language language = new Language { Name = "en", DisplayName = "English" };
            languageService.AddLanguage(language);

            Site siteInput = new Site
            {
                Name = "Oxite Site",
                DisplayName = "My Oxite Site",
                Description = "An Oxite based web site.",
                FavIconUrl = "/Content/icons/flame.ico",
                LanguageDefault = "en",
                PageTitleSeparator = "-",
                GravatarDefault = "http://mschnlnine.vo.llnwd.net/d1/oxite/gravatar.jpg",
                SkinsPath = "/Skins",
                SkinsScriptsPath = "/Scripts",
                SkinsStylesPath = "/Styles",
                Skin = "Default",
                AdminSkin = "Admin",
                RouteUrlPrefix = string.Empty,
                PluginsPath = "/Plugins",
                CommentStateDefault = "PendingApproval",
                Host = new Uri("http://localhost:30913"),
                TimeZoneOffset = -8,
                PostEditTimeout = 24,
                IncludeOpenSearch = true,
                AuthorAutoSubscribe = true
            };

            siteService.AddSite(siteInput, out validationState, out site);

            userService.EnsureAnonymousUser();

            return site;
        }

        private ModelResult<UserAuthenticated> SetupUser(UserInputAdd userInput, ServiceContext serviceContext)
        {
            return this.userService.AddUser(userInput, serviceContext);
        }

    }
}
