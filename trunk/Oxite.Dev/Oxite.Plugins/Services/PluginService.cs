﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Transactions;
using System.Web.Routing;
using Oxite.Infrastructure;
using Oxite.Models;
using Oxite.Modules.Plugins.Extensions;
using Oxite.Modules.Plugins.Models;
using Oxite.Modules.Plugins.Repositories;
using Oxite.Plugins;
using Oxite.Plugins.Extensions;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.Plugins.Services
{
    public class PluginService : IPluginService
    {
        private readonly IPluginRepository repository;
        private readonly IValidationService validator;
        private readonly IPluginEngine pluginEngine;
        private readonly IOxiteCacheModule cache;
        private readonly ISiteService siteService;
        private readonly IModulesLoaded modules;
        private readonly PluginTemplateRegistry pluginTemplateRegistry;
        private readonly RouteCollection routes;
        private readonly PluginStyleRegistry pluginStyleRegistry;
        private readonly PluginScriptRegistry pluginScriptRegistry;

        public PluginService(IPluginRepository repository, IValidationService validator, IPluginEngine pluginEngine, IModulesLoaded modules, ISiteService siteService, PluginTemplateRegistry pluginTemplateRegistry, RouteCollection routes, PluginStyleRegistry pluginStyleRegistry, PluginScriptRegistry pluginScriptRegistry)
        {
            this.repository = repository;
            this.validator = validator;
            this.pluginEngine = pluginEngine;
            this.cache = modules.GetModules<IOxiteCacheModule>().Reverse().First();
            this.siteService = siteService;
            this.modules = modules;
            this.pluginTemplateRegistry = pluginTemplateRegistry;
            this.routes = routes;
            this.pluginStyleRegistry = pluginStyleRegistry;
            this.pluginScriptRegistry = pluginScriptRegistry;
        }

        #region IPluginService Members

        public IEnumerable<Plugin> GetPlugins()
        {
            return repository.GetPlugins(getSiteID()).ToArray();
        }

        public Plugin GetPlugin(PluginAddress pluginAddress)
        {
            return repository.GetPlugin(getSiteID(), pluginAddress.PluginID);
        }

        public Plugin InstallPlugin(PluginInstallInput pluginInstallInput, bool? overrideEnabled)
        {
            PluginContainer pluginContainer = null;

            if (!string.IsNullOrEmpty(pluginInstallInput.VirtualPath))
                pluginContainer = pluginEngine.GetPlugins(p => (p.Tag == null || !(p.Tag is Plugin)) && p.CompilationAssembly != null && string.Compare(p.VirtualPath, pluginInstallInput.VirtualPath, true) == 0).FirstOrDefault();

            if (pluginContainer == null) throw new InvalidOperationException("Could not find plugin to install");

            ValidationStateDictionary validationState = ValidatePlugin(pluginContainer);
            Plugin plugin = repository.Save(new Plugin(getSiteID(), pluginInstallInput.VirtualPath, validationState.IsValid && overrideEnabled.HasValue ? overrideEnabled.Value : validationState.IsValid, pluginContainer.GetPropertiesUsingDefaultValues()));

            plugin.Container = pluginContainer;
            pluginContainer.Tag = plugin;
            pluginContainer.IsValid = validationState.IsValid;

            pluginTemplateRegistry.Reload(pluginEngine);
            routes.Reload(modules, this, pluginEngine);
            pluginStyleRegistry.Reload(pluginEngine);
            pluginScriptRegistry.Reload(pluginEngine);

            return plugin;
        }

        public ValidationStateDictionary ValidatePlugin(PluginContainer pluginContainer)
        {
            IEnumerable<ExtendedProperty> extendedProperties = pluginContainer.GetPropertiesUsingDefaultValues();

            return validatePluginPropertyValues(new PluginPropertiesInput(pluginContainer.Definitions, extendedProperties, name => pluginContainer.GetPropertyDefinitions(name), name => extendedProperties.First(ep => string.Compare(ep.Name, name, true) == 0).Value));
        }

        private ValidationStateDictionary validatePluginPropertyValues(Plugin plugin, NameValueCollection newPropertyValues)
        {
            return validatePluginPropertyValues(new PluginPropertiesInput(plugin, newPropertyValues));
        }

        private ValidationStateDictionary validatePluginPropertyValues(PluginPropertiesInput input)
        {
            ValidationStateDictionary validationState = new ValidationStateDictionary();

            validationState.Add(typeof(PluginPropertiesInput), validator.Validate(input));

            return validationState;
        }

        public ModelResult<Plugin> EditPlugin(PluginAddress pluginAddress, PluginEditInput pluginEditInput, bool hasCodeChanges)
        {
            Plugin originalPlugin = GetPlugin(pluginAddress);
            bool enabled = originalPlugin.Enabled;
            bool newlyEnabled = false;
            Plugin newPlugin;
            PluginContainer pluginContainer;
            ValidationStateDictionary validationState;

            if (!hasCodeChanges)
            {
                validationState = validatePluginPropertyValues(originalPlugin.FillContainer(pluginEngine), pluginEditInput.PropertyValues);

                pluginContainer = pluginEngine.GetPlugin(originalPlugin);
                pluginContainer.IsValid = validationState.IsValid;

                newlyEnabled = !enabled && validationState.IsValid;

                newPlugin = originalPlugin.Apply(pluginEditInput, newlyEnabled ? (bool?)true : null);

                if (!validationState.IsValid) return new ModelResult<Plugin>(newPlugin, validationState);
            }
            else
            {
                validationState = new ValidationStateDictionary();

                originalPlugin.SaveFileText(pluginEditInput.Code);

                pluginContainer = pluginEngine.ReloadPlugin(p => p.Tag != null && p.Tag is Plugin && ((Plugin)p.Tag).ID == originalPlugin.ID, p => originalPlugin.VirtualPath);

                if (pluginContainer.IsLoaded)
                    newPlugin = originalPlugin.ApplyNew(pluginEditInput, pluginContainer.GetProperties());
                else
                    newPlugin = originalPlugin.Apply(pluginEditInput, null);
            }

            newPlugin = repository.Save(newPlugin);

            newPlugin.Container = pluginContainer;
            pluginContainer.Tag = newPlugin;

            if (!hasCodeChanges)
            {
                pluginContainer.ApplyProperties(newPlugin.ExtendedProperties);

                if (newlyEnabled)
                {
                    pluginTemplateRegistry.Reload(pluginEngine);
                    routes.Reload(modules, this, pluginEngine);
                    pluginStyleRegistry.Reload(pluginEngine);
                    pluginScriptRegistry.Reload(pluginEngine);
                }
                else
                {
                    pluginTemplateRegistry.UpdatePlugin(newPlugin);
                    pluginStyleRegistry.UpdatePlugin(newPlugin);
                    pluginScriptRegistry.UpdatePlugin(newPlugin);
                }
            }
            else
            {
                if (pluginContainer.HasMethod("ProcessDisplayOfPost"))
                    cache.Invalidate<Post>();
                else if (pluginContainer.HasMethod("ProcessDisplayOfPage"))
                    cache.Invalidate<Page>();
                else if (pluginContainer.HasMethod("ProcessDisplayOfComment"))
                    cache.Invalidate<Comment>();

                pluginTemplateRegistry.Reload(pluginEngine);
                routes.Reload(modules, this, pluginEngine);
                pluginStyleRegistry.Reload(pluginEngine);
                pluginScriptRegistry.Reload(pluginEngine);
            }

            return new ModelResult<Plugin>(newPlugin, validationState);
        }

        public void SetPluginEnabled(PluginAddress pluginAddress, bool enabled)
        {
            Plugin plugin = GetPlugin(pluginAddress);

            if (plugin != null && plugin.Enabled != enabled)
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    repository.SetEnabled(getSiteID(), plugin.ID, enabled);

                    plugin = GetPlugin(pluginAddress);

                    PluginContainer pluginContainer = pluginEngine.GetPlugins(p => p.Tag != null && p.Tag is Plugin && ((Plugin)p.Tag).ID == plugin.ID).FirstOrDefault();

                    plugin.Container = pluginContainer;
                    pluginContainer.Tag = plugin;

                    pluginTemplateRegistry.Reload(pluginEngine);
                    routes.Reload(modules, this, pluginEngine);
                    pluginStyleRegistry.Reload(pluginEngine);
                    pluginScriptRegistry.Reload(pluginEngine);

                    transaction.Complete();
                }
            }
        }

        public void UninstallPlugin(PluginAddress pluginAddress, ServiceContext context)
        {
            Plugin plugin = GetPlugin(pluginAddress);

            if (plugin != null)
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    repository.Remove(getSiteID(), plugin.ID);

                    PluginContainer pluginContainer = pluginEngine.GetPlugins(p => p.Tag != null && p.Tag is Plugin && ((Plugin)p.Tag).ID == plugin.ID).FirstOrDefault();

                    if (!System.IO.File.Exists(context.HttpContext.Server.MapPath(pluginContainer.VirtualPath)))
                        pluginEngine.RemovePlugin(pluginContainer);

                    plugin.Container = null;
                    pluginContainer.Tag = null;

                    pluginTemplateRegistry.Reload(pluginEngine);
                    routes.Reload(modules, this, pluginEngine);
                    pluginStyleRegistry.Reload(pluginEngine);
                    pluginScriptRegistry.Reload(pluginEngine);

                    transaction.Complete();
                }
            }
        }

        public void ReloadPlugin(PluginAddress pluginAddress)
        {
            Plugin plugin = GetPlugin(pluginAddress);

            if (plugin != null)
            {
                PluginContainer pluginContainer = pluginEngine.ReloadPlugin(p => p.Tag != null && p.Tag is Plugin && ((Plugin)p.Tag).ID == plugin.ID, p => plugin.VirtualPath);

                if (pluginContainer.IsLoaded)
                {
                    plugin = repository.Save(plugin.ApplyProperties(pluginContainer.GetPropertiesUsingDefaultValues()));

                    pluginContainer.ApplyProperties(plugin.ExtendedProperties);
                }

                plugin.Container = pluginContainer;
                pluginContainer.Tag = plugin;

                pluginTemplateRegistry.Reload(pluginEngine);
                routes.Reload(modules, this, pluginEngine);
                pluginStyleRegistry.Reload(pluginEngine);
                pluginScriptRegistry.Reload(pluginEngine);
            }
        }

        public void ReorderPluginOperation(Guid pluginID, string operationType, string operation, int newSequenceNumber)
        {
            throw new NotImplementedException();
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
