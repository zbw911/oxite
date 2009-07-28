//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Linq;
using System.Web.Mvc;
using Oxite.Extensions;
using Oxite.Models;
using Oxite.Modules.CMS.Extensions;
using Oxite.Modules.CMS.Models;
using Oxite.Modules.CMS.Services;
using Oxite.Services;
using Oxite.Validation;
using Oxite.ViewModels;

namespace Oxite.Modules.CMS.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageService pageService;

        public PageController(IPageService pageService)
        {
            this.pageService = pageService;
            ValidateRequest = false;
        }

        public OxiteViewModelItems<Page> List(UserAuthenticated currentUser)
        {
            return new OxiteViewModelItems<Page>
            {
                Items = pageService.GetPages()
            };
        }

        public OxiteViewModelItems<Page> ListByParent(PageAddress pageAddress, UserAuthenticated currentUser)
        {
            //todo: (nheskew) use the parent to get child pages
            Page parentPage = pageService.GetPage(pageAddress, new ServiceContext(ControllerContext, currentUser));

            return new OxiteViewModelItems<Page>
            {
                Items = pageService.GetPages()
            };
        }

        public OxiteViewModelItem<Page> Item(PageAddress pageAddress, UserAuthenticated currentUser)
        {
            Page page = pageService.GetPage(pageAddress, new ServiceContext(ControllerContext, currentUser));

            if (page == null) return null;

            //TODO: (erikpo) Check permissions to see if the current user is allowed to see this page or not

            return new OxiteViewModelItem<Page>
            {
                Item = page
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validate(PageInput pageInput)
        {
            ValidationStateDictionary validationState = pageService.ValidatePageInput(pageInput);

            if (validationState.IsValid) return Content("");

            return PartialView("ValidationErrors", new OxiteViewModelPartial<ValidationStateDictionary>(new OxiteViewModel(), validationState));
        }

        [ActionName("ItemAdd"), AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<PageInput> Add(PageAddress parentPageAddress, PageInput pageInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            Page page = parentPageAddress.PagesInPath.Count() > 0 ? pageService.GetPage(parentPageAddress, new ServiceContext(ControllerContext, currentUser)) : null;

            return new OxiteViewModelItem<PageInput>
            {
                Item = page == null || pageInput.ParentID != Guid.Empty ? pageInput : new PageInput(page.ID, pageInput.Title, pageInput.Body, pageInput.Slug, pageInput.Published)
            };
        }

        [ActionName("ItemAdd"), AcceptVerbs(HttpVerbs.Post)]
        public object AddSave(PageAddress parentPageAddress, PageInput pageInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<Page> results = pageService.AddPage(parentPageAddress, pageInput, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return Add(parentPageAddress, pageInput, currentUser);
            }

            return Redirect(Url.AppPath(Url.Page(results.Item)));
        }

        [ActionName("ItemEdit"), AcceptVerbs(HttpVerbs.Get)]
        public OxiteViewModelItem<PageInput> Edit(PageAddress pageAddress, PageInput pageInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            Page page = pageService.GetPage(pageAddress, new ServiceContext(ControllerContext, currentUser));

            if (page == null) return null;

            var model = new OxiteViewModelItem<PageInput>
            {
                Item = new PageInput(page, pageInput)
            };

            model.AddModelItem(pageService.GetPage(pageAddress, new ServiceContext(ControllerContext, currentUser)));

            return model;
        }

        [ActionName("ItemEdit"), AcceptVerbs(HttpVerbs.Post)]
        public object SaveEdit(PageAddress pageAddress, PageInput pageInput, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            ModelResult<Page> results = pageService.EditPage(pageAddress, pageInput, new ServiceContext(ControllerContext, currentUser));

            if (!results.IsValid)
            {
                ModelState.AddModelErrors(results.ValidationState);

                return Edit(pageAddress, pageInput, currentUser);
            }

            return Redirect(Url.AppPath(Url.Page(results.Item)));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Remove(PageAddress pageAddress, string returnUri, UserAuthenticated currentUser)
        {
            //TODO: (erikpo) Check permissions

            pageService.RemovePage(pageAddress, new ServiceContext(ControllerContext, currentUser));

            return Redirect(returnUri);
        }
    }
}
