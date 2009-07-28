//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;
using Microsoft.Practices.Unity;
using System;
using System.Linq;

namespace Oxite.Infrastructure
{
    public class ModulesLoaded : IModulesLoaded
    {
        private readonly IUnityContainer container;
        private readonly List<IOxiteModule> modules;

        public ModulesLoaded(IUnityContainer container)
        {
            this.container = container;
            modules = new List<IOxiteModule>(10);
        }

        #region IModulesLoaded Members

        public IOxiteModule Load(Module module)
        {
            if (module == null) return null;

            Type type = Type.GetType(module.Type, true);
            IOxiteModule moduleInstance = container.Resolve(type) as IOxiteModule;

            modules.Add(moduleInstance);

            return moduleInstance;
        }

        public IEnumerable<IOxiteModule> GetModules()
        {
            return GetModules<IOxiteModule>();
        }

        public IEnumerable<T> GetModules<T>() where T : IOxiteModule
        {
            return modules.Where(m => m is T).Cast<T>();
        }

        public void UnloadModules()
        {
            foreach (IOxiteModule module in modules)
                module.Unload();

            modules.Clear();
        }

        #endregion
    }
}
