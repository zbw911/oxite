//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using System.Web.Mvc;
using Oxite.Plugins;
using Oxite.ViewModels;

namespace Oxite.Extensions
{
    public static class ViewDataDictionaryExtensions
    {
        public static OxiteViewModel GetOxiteViewModel(this ViewDataDictionary viewData)
        {
            OxiteViewModel model = null;

            if (viewData.Model != null)
            {
                model = viewData.Model as OxiteViewModel;

                if (model != null) return model;

                Type type = viewData.Model.GetType();

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(OxiteViewModelPartial<>))
                    model = type.GetProperties().Where(p => p.Name == "RootModel").Single().GetValue(viewData.Model, null) as OxiteViewModel;
            }

            return model;
        }
    }
}
