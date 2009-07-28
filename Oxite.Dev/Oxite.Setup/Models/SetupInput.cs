//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------

namespace Oxite.Modules.Setup.Models
{
    using System;
    using Oxite.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Class to contain all basic data needed during the initial site setup.
    /// </summary>
    public class SetupInput
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the SetupInput class.
        /// </summary>
        public SetupInput()
        {
            this.SiteType = SiteType.PersonalBlog;
            this.Modules = new List<Module>();
        }

        /// <summary>
        /// Initializes a new instance of the SetupInput class.
        /// </summary>
        /// <param name="siteID">Id of the site.</param>
        /// <param name="blogName">Name of the default blog for the site.</param>
        /// <param name="blogDisplayName">Display name for the default blog for the site.</param>
        /// <param name="description">Description of the default blog for the site.</param>
        /// <param name="commentingDisabled">Determines whether or not commenting is disabled for the default blog.</param>
        /// <param name="userName">User name for the initial admin account.</param>
        /// <param name="adminDisplayName">Display name for the initial admin account.</param>
        /// <param name="email">Email for the initial admin account.</param>
        /// <param name="password">Password for the initial admin account.</param>
        /// <param name="passwordConfirm">Password confirmation entry for the initial admin account.</param>
        public SetupInput(Guid siteID, string blogName, string blogDisplayName, string description, bool commentingDisabled, string userName, string adminDisplayName, string email, string password, string passwordConfirm) : this()
        {
            this.SiteID = siteID;
            this.BlogName = blogName;
            this.BlogDisplayName = blogDisplayName;
            this.Description = description;
            this.CommentingDisabled = commentingDisabled;

            this.UserName = userName;
            this.AdminDisplayName = adminDisplayName;
            this.Email = email;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Id of the site.
        /// </summary>
        public Guid SiteID { get; private set; }

        /// <summary>
        /// Name of the default blog for the site.
        /// </summary>
        public string BlogName { get; private set; }

        /// <summary>
        /// Display name for the default blog for the site.
        /// </summary>
        public string BlogDisplayName { get; private set; }

        /// <summary>
        /// Description of the default blog for the site.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Determines whether or not commenting is disabled for the default blog.
        /// </summary>
        public bool CommentingDisabled { get; private set; }

        /// <summary>
        /// User name for the initial admin account.
        /// </summary>
        public string UserName { get; private set; }
        
        /// <summary>
        /// Display name for the initial admin account.
        /// </summary>
        public string AdminDisplayName { get; private set; }
        
        /// <summary>
        /// Email for the initial admin account.
        /// </summary>
        public string Email { get; private set; }
        
        /// <summary>
        /// Password for the initial admin account.
        /// </summary>
        public string Password { get; private set; }
        
        /// <summary>
        /// Password confirmation entry for the initial admin account.
        /// </summary>
        public string PasswordConfirm { get; private set; }

        public SiteType SiteType { get; set; }

        public List<Module> Modules { get; set; }
        #endregion
    }
}
