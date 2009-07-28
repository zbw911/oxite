//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using Oxite.Infrastructure;
using Oxite.Modules.CMS.Models;
using Oxite.Services;
using Oxite.Validation;

namespace Oxite.Modules.CMS.Validation
{
    public class PageInputValidator : ValidatorBase<PageInput>
    {
        public PageInputValidator(ILocalizationService localizationService, ISiteService siteService, IRegularExpressions expressions)
            : base(localizationService, siteService, expressions) { }

        #region IValidator Members

        public override ValidationState Validate(PageInput input)
        {
            if (input == null) throw new ArgumentNullException("input");

            ValidationState validationState = new ValidationState();

            string title = input.Title.Trim();
            if (string.IsNullOrEmpty(title))
            {
                validationState.Errors.Add(CreateValidationError(title, "Title", "Title.RequiredError", "Title is not set."));
            }
            else
            {
                if (title.Length > 250)
                {
                    validationState.Errors.Add(CreateValidationError(title, "Title", "Title.MaxLengthExceededError", "Title must be {0} characters or less.", 250));
                }
            }

            string body = input.Body.Trim();
            if (string.IsNullOrEmpty(body))
            {
                validationState.Errors.Add(CreateValidationError(body, "Body", "Body.RequiredError", "Body is not set."));
            }

            string slug = input.Slug.Trim();
            if (string.IsNullOrEmpty(slug))
            {
                validationState.Errors.Add(CreateValidationError(slug, "Slug", "Slug.RequiredError", "Slug is not set."));
            }
            else
            {
                if (!Expressions.IsMatch("IsSlug", slug))
                {
                    validationState.Errors.Add(CreateValidationError(slug, "Slug", "Slug.NotValidError", "Slug is not valid."));
                }
            }

            return validationState;
        }

        #endregion
    }
}
