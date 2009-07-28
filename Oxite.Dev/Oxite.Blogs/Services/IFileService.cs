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
    public interface IFileService
    {
        IEnumerable<File> GetFiles(PostAddress postAddress);
        ModelResult<File> AddFile(PostAddress postAddress, FileInput fileInput);
        ModelResult<File> AddFile(PostAddress postAddress, FileContentInput fileInput);
        ModelResult<File> EditFile(PostAddress postAddress, FileAddress fileAddress, FileInput fileInput);
        ModelResult<File> EditFile(PostAddress postAddress, FileAddress fileAddress, FileContentInput fileInput);
        bool RemoveFile(PostAddress postAddress, FileAddress fileAddress);
    }
}
