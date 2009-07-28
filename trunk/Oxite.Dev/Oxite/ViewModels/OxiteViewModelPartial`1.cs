//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------

namespace Oxite.ViewModels
{
    public class OxiteViewModelPartial<T>
    {
        public OxiteViewModelPartial(OxiteViewModel rootModel, T partialModel)
        {
            RootModel = rootModel;
            PartialModel = partialModel;
        }

        public OxiteViewModel RootModel { get; set; }
        public T PartialModel { get; set; }
    }
}
