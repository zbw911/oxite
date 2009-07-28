//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Blogs.Services;
using Oxite.Services;
using Oxite.ViewModels;

namespace Oxite.Modules.Blogs.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileService fileService;
        private readonly IPostService postService;

        public FileController(IFileService fileService, IPostService postService)
        {
            this.fileService = fileService;
            this.postService = postService;
        }

        public OxiteViewModelItems<File> ListByPost(PostAddress postAddress, UserAuthenticated currentUser)
        {
            Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

            if (post == null) return null;

            IEnumerable<File> files = fileService.GetFiles(postAddress);

            return new OxiteViewModelItems<File> { Container = post, Items = files };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object AddFileContentToPost(PostAddress postAddress, FileContentInput fileInput, string returnUri, UserAuthenticated currentUser)
        {
            return saveFileToPost(postAddress, () => addFileContent(postAddress, fileInput), returnUri, currentUser);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object AddFileToPost(PostAddress postAddress, FileInput fileInput, string returnUri, UserAuthenticated currentUser)
        {
            return saveFileToPost(postAddress, () => fileService.AddFile(postAddress, fileInput), returnUri, currentUser);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object EditFileContentOnPost(PostAddress postAddress, FileAddress fileAddress, FileContentInput fileInput, string returnUri, UserAuthenticated currentUser)
        {
            return saveFileToPost(postAddress, () => editFileContent(postAddress, fileAddress, fileInput), returnUri, currentUser);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public object EditFileOnPost(PostAddress postAddress, FileAddress fileAddress, FileInput fileInput, string returnUri, UserAuthenticated currentUser)
        {
            return saveFileToPost(postAddress, () => fileService.EditFile(postAddress, fileAddress, fileInput), returnUri, currentUser);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RemoveFileFromPost(PostAddress postAddress, FileAddress fileAddress, string returnUri)
        {
            bool removedFile = fileService.RemoveFile(postAddress, fileAddress);

            if (!string.IsNullOrEmpty(returnUri))
                return Redirect(returnUri);

            return new JsonResult { Data = removedFile };
        }

        private object saveFileToPost(PostAddress postAddress, Func<ModelResult<File>> saveFile, string returnUri, UserAuthenticated currentUser)
        {
            ModelResult<File> results = saveFile();

            if (!string.IsNullOrEmpty(returnUri))
            {
                if (results.IsValid)
                    return new RedirectResult(returnUri);
                else
                {
                    ModelState.AddModelErrors(results.ValidationState);

                    return ListByPost(postAddress, currentUser);
                }
            }

            if (results.IsValid)
            {
                Post post = postService.GetPost(postAddress, new ServiceContext(ControllerContext, currentUser));

                return PartialView("ManageFile", new OxiteViewModelItem<File> { Container = post, Item = results.Item });
            }

            return new JsonResult { Data = false };
        }

        private ModelResult<File> addFileContent(PostAddress postAddress, FileContentInput fileInput)
        {
            //TODO: (erikpo) Add file to the file system if possible

            return fileService.AddFile(postAddress, fileInput);
        }

        private ModelResult<File> editFileContent(PostAddress postAddress, FileAddress fileAddress, FileContentInput fileInput)
        {
            //TODO: (erikpo) Edit file on the file system if possible

            return fileService.EditFile(postAddress, fileAddress, fileInput);
        }
    }
}
