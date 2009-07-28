//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;

namespace Oxite.Models
{
    public class PluginComment
    {
        private readonly Comment original;

        public PluginComment(Comment comment)
        {
            original = comment;

            Post = comment.Post;
            Parent = comment.Parent;
            CreatorUserID = comment.CreatorUserID;
            CreatorName = comment.CreatorName;
            CreatorEmail = comment.CreatorEmail;
            CreatorEmailHash = comment.CreatorEmailHash;
            CreatorUrl = comment.CreatorUrl;
            CreatorIP = comment.CreatorIP;
            CreatorUserAgent = comment.CreatorUserAgent;
            Language = comment.Language;
            Body = comment.Body;
            Slug = comment.Slug;
            State = comment.State;
            Created = comment.Created;
            Modified = comment.Modified;
        }

        public PostSmall Post { get; private set; }
        public CommentSmall Parent { get; private set; }
        public Guid CreatorUserID { get; set; }
        public string CreatorName { get; set; }
        public string CreatorEmail { get; set; }
        public string CreatorEmailHash { get; set; }
        public string CreatorUrl { get; set; }
        public long CreatorIP { get; private set; }
        public string CreatorUserAgent { get; private set; }
        public Language Language { get; private set; }
        public string Body { get; set; }
        public string Slug { get; set; }
        public EntityState State { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime Modified { get; private set; }

        public Comment ToComment()
        {
            return new Comment(Body, original.Created, CreatorUserID, CreatorName, CreatorEmail, CreatorEmailHash, CreatorUrl, original.CreatorIP, original.CreatorUserAgent, original.ID, original.Language, original.Modified, original.Parent, original.Post, Slug, original.State);
        }
    }
}
