//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;

namespace Oxite.Modules.CMS.Models
{
    public class PluginPageInput
    {
        private readonly PageInput originalInput;

        public PluginPageInput(PageInput pageInput)
        {
            originalInput = pageInput;

            Title = pageInput.Title;
            Body = pageInput.Body;
            Slug = pageInput.Slug;
            Published = pageInput.Published.HasValue && pageInput.Published.Value <= DateTime.Now;
            if (Published)
                PublishedDate = pageInput.Published.Value;
        }

        public string Title { get; set; }
        public string Body { get; set; }
        public string Slug { get; set; }
        public bool Published { get; private set; }
        public DateTime PublishedDate { get; private set; }

        public PageInput ToPageInput()
        {
            return new PageInput(originalInput.ParentID, Title, Body, Slug, originalInput.Published);
        }
    }
}
