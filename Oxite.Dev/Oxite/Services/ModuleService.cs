//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Oxite.Models;
using Oxite.Repositories;
using Oxite.Services;

namespace Oxite.Services
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository repository;
        private readonly ISiteService siteService;

        public ModuleService(IModuleRepository repository, ISiteService siteService)
        {
            this.repository = repository;
            this.siteService = siteService;
        }

        #region IModuleService Members

        public IEnumerable<Module> GetModules()
        {
            return repository.GetModules(getSiteID()).ToArray();
        }

        #endregion

        #region Private Methods

        private Guid getSiteID()
        {
            return siteService.GetSite().ID;
        }

        #endregion
    }
}
