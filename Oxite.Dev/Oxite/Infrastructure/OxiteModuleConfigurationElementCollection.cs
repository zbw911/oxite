﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Configuration;

namespace Oxite.Infrastructure
{
    public class OxiteModuleConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get
            {
                return (bool)this["enabled"];
            }
            set
            {
                this["enabled"] = value;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new OxiteModuleConfigurationElement();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new OxiteModuleConfigurationElement(elementName);
        }

        protected new OxiteModuleConfigurationElement BaseGet(int index)
        {
            return (OxiteModuleConfigurationElement)base.BaseGet(index);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OxiteModuleConfigurationElement)element).Name;
        }
    }
}
