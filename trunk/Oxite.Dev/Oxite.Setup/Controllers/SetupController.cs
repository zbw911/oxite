//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
namespace Oxite.Modules.Setup.Controllers
{
    using System;
    using System.Web.Mvc;
    using Oxite.Extensions;
    using Oxite.Models;
    using Oxite.Modules.Blogs.Models;
    using Oxite.Modules.Membership.Models;
    using Oxite.Modules.Setup.Models;
    using Oxite.Modules.Setup.Services;
    using Oxite.Services;
    using Oxite.Validation;
    using Oxite.ViewModels;

    /// <summary>
    /// Controller to manage actions related to Oxite site setup
    /// </summary>
    public class SetupController : Controller
    {
        #region Fields
        /// <summary>
        /// Setup service to use to setup basic information about the site.
        /// </summary>
        private readonly ISetupService setupService;

        /// <summary>
        /// Site service to retrieve details of the site.
        /// </summary>
        private readonly ISiteService siteService;

        private readonly IModuleService moduleService;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the SetupController class.
        /// </summary>
        /// <param name="setupService">Setup service to use to setup basic information about the site.</param>
        /// <param name="siteService">Site service to retrieve details of the site.</param>
        public SetupController(ISetupService setupService, ISiteService siteService, IModuleService moduleService)
        {
            this.setupService = setupService;
            this.siteService = siteService;
            this.moduleService = moduleService;
        }
        #endregion

        /// <summary>
        /// Display the view that allows the user to update the Admin account settings for
        /// the new site.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>OxiteViewModelItem with its Item property set to the current SetupInput instance.</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public virtual object BasicSettings(SetupInput input)
        {
            foreach (Module module in this.moduleService.GetModules())
            {
                input.Modules.Add(module);
            }

            return new OxiteViewModelItem<SetupInput>() { Item = input };
        }

        /// <summary>
        /// Create site based on user input.  Includes creating site entry, Admin account, anonymous user account,
        /// and default blog.  If setup is successful, navigate to success page.  Otherwise, return to page with
        /// error messaging.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>RedirectView to SetupComplete action if successful.  BasicSettings view otherwise.</returns>
        [ActionName("BasicSettings")]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual object BasicSettingsSave(SetupInput input)
        {
            ValidationStateDictionary validationState;

            ServiceContext context = new ServiceContext(this.ControllerContext);

            Site site = this.setupService.SetupSite(input, context, out validationState);

            if (!validationState.IsValid)
            {
                ModelState.AddModelErrors(validationState);

                return this.BasicSettings(input);
            }

            return RedirectToAction("SetupComplete", site);
        }

        /// <summary>
        /// Display the view that tells the user the setup completed successfully and gives them the choice between
        /// further defining site information or just going to their site.
        /// </summary>
        /// <param name="site">Site object containing details of newly created site.</param>
        /// <returns>OxiteViewModelItem that has its Item property set to the new Site.</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public virtual object SetupComplete(Site site)
        {
            return new OxiteViewModelItem<Site>() { Item = site };
        }

        /// <summary>
        /// Display the view that allows the user to select the way that post/page/etc data
        /// will be stored for the new site.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>OxiteViewModelItem with its Item property set to the current SetupInput instance.</returns>
        [ActionName("SiteType")]
        [AcceptVerbs(HttpVerbs.Get)]
        public virtual object SiteType(SetupInput input)
        {
            Site site = this.siteService.GetSite();

            if (site != null)
            {
                // throw new InvalidOperationException("A site has already been set up.  Please contact the administrator to make changes.");
            }

            return new OxiteViewModelItem<SetupInput>() { Item = input };
        }

        // TODO: This should likely do something useful.
        /// <summary>
        /// This is where the logic to setup whatever storage mechanism should go.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>Redirects to the screen to setup basic settings if successful.  Storage view otherwise.</returns>
        [ActionName("SiteType")]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual object SiteTypeSave(SetupInput input)
        {
            return RedirectToAction("Storage", input);
        }

        /// <summary>
        /// Display the view that allows the user to select the way that post/page/etc data
        /// will be stored for the new site.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>OxiteViewModelItem with its Item property set to the current SetupInput instance.</returns>
        [ActionName("Storage")]
        [AcceptVerbs(HttpVerbs.Get)]
        public virtual object Storage(SetupInput input)
        {
            return new OxiteViewModelItem<SetupInput>() { Item = input };
        }

        // TODO: This should likely do something useful.
        /// <summary>
        /// This is where the logic to setup whatever storage mechanism should go.
        /// </summary>
        /// <param name="input">SetupInput instance containing all user entered information about the site and its
        /// components.</param>
        /// <returns>Redirects to the screen to setup basic settings if successful.  Storage view otherwise.</returns>
        [ActionName("Storage")]
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual object StorageSave(SetupInput input)
        {
            return RedirectToAction("BasicSettings", input);
        }
    }
}
