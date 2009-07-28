﻿//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxite.Models
{
    public class ImportPostInput
    {
        public ImportPostInput(string body, string bodyShort, bool commentingDisabled, DateTime created, UserAuthenticated creator, DateTime modified, DateTime? published, string slug, EntityState state, IEnumerable<Tag> tags, string title, IEnumerable<Trackback> trackbacks)
        {
            Body = body;
            BodyShort = bodyShort;
            CommentingDisabled = commentingDisabled;
            Created = created;
            Creator = creator;
            Modified = modified;
            Published = published;
            Slug = slug;
            Tags = tags;
            Title = title;
            Trackbacks = trackbacks;
        }

        public string Body { get; private set; }
        public string BodyShort { get; private set; }
        public bool CommentingDisabled { get; private set; }
        public DateTime Created { get; private set; }
        public UserAuthenticated Creator { get; private set; }
        public DateTime Modified { get; private set; }
        public DateTime? Published { get; private set; }
        public string Slug { get; private set; }
        public EntityState State { get; private set; }
        public IEnumerable<Tag> Tags { get; private set; }
        public string Title { get; private set; }
        public IEnumerable<Trackback> Trackbacks { get; private set; }

        public Post ToPost(Blog blog)
        {
            return new Post(blog, Body, BodyShort, CommentingDisabled, Created, Creator, Guid.Empty, 0, Modified, Published, Slug, State, Tags, Title, Trackbacks);
        }
    }
}
