// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    public class SpamCandidate
    {
        public Uri blog { get; set; }
        public string user_ip { get; set; }
        public string user_agent { get; set; }
        public Uri referrer { get; set; }
        public Uri permalink { get; set; }
        public string comment_type { get; set; }
        public string comment_author { get; set; }
        public string comment_author_email { get; set; }
        public string comment_author_url { get; set; }
        public string comment_content { get; set; }
    }
}