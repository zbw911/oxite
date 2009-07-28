//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Collections.Generic;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;

namespace Oxite.Modules.Blogs.Services
{
    public class FileService : IFileService
    {
        #region IFileService Members

        public IEnumerable<File> GetFiles(PostAddress postAddress)
        {
            throw new System.NotImplementedException();
        }

        public ModelResult<File> AddFile(PostAddress postAddress, FileInput fileInput)
        {
            throw new System.NotImplementedException();
        }

        public ModelResult<File> AddFile(PostAddress postAddress, FileContentInput fileInput)
        {
            throw new System.NotImplementedException();
        }

        public ModelResult<File> EditFile(PostAddress postAddress, FileAddress fileAddress, FileInput fileInput)
        {
            throw new System.NotImplementedException();
        }

        public ModelResult<File> EditFile(PostAddress postAddress, FileAddress fileAddress, FileContentInput fileInput)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveFile(PostAddress postAddress, FileAddress fileAddress)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
