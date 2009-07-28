//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Runtime.Serialization;
using Oxite.Infrastructure;
using System.Collections.Generic;

namespace Oxite.Models
{
    [DataContract]
    public class Comment : EntityBase, ICacheItem
    {
        private readonly UserAnonymous userAnonymous;
        private readonly UserAuthenticated userAuthenticated;

        public Comment(Guid id)
            : base(id)
        {
        }

        public Comment(string body, Guid creatorUserID, string creatorName, string creatorEmail, string creatorEmailHash, string creatorUrl, long creatorIP, string creatorUserAgent, Language language, CommentSmall parent, string slug, EntityState state)
        {
            Body = body;
            CreatorUserID = creatorUserID;
            CreatorName = creatorName;
            CreatorEmail = creatorEmail;
            CreatorEmailHash = creatorEmailHash;
            CreatorUrl = creatorUrl;
            CreatorIP = creatorIP;
            CreatorUserAgent = creatorUserAgent;
            Parent = parent;
            Slug = slug;
            Language = language;
            State = state;
        }

        public Comment(string body, DateTime created, UserAnonymous creator, long creatorIP, string creatorUserAgent, Language language, DateTime modified, string slug, EntityState state)
            : this(body, Guid.Empty, creator.Name, creator.Email, creator.EmailHash, creator.Url, creatorIP, creatorUserAgent, language, null, slug, state)
        {
            Created = created;
            userAnonymous = creator;
        }

        public Comment(string body, DateTime created, UserAuthenticated creator, long creatorIP, string creatorUserAgent, Language language, DateTime modified, string slug, EntityState state)
            : this(body, creator.ID, null, null, null, null, creatorIP, creatorUserAgent, language, null, slug, state)
        {
            Created = created;
            userAuthenticated = creator;
        }

        public Comment(string body, DateTime created, UserAnonymous creator, long creatorIP, string creatorUserAgent, Guid id, Language language, DateTime modified, CommentSmall parent, PostSmall post, string slug, EntityState state)
            : this(id)
        {
            Body = body;
            Created = created;
            CreatorName = creator.Name;
            CreatorEmail = creator.Email;
            CreatorEmailHash = creator.EmailHash;
            CreatorUrl = creator.Url;
            CreatorIP = creatorIP;
            CreatorUserAgent = creatorUserAgent;
            Language = language;
            Parent = parent;
            Post = post;
            Slug = slug;
            State = state;

            userAnonymous = creator;
        }

        public Comment(string body, DateTime created, UserAuthenticated creator, long creatorIP, string creatorUserAgent, Guid id, Language language, DateTime modified, CommentSmall parent, PostSmall post, string slug, EntityState state)
            : this(id)
        {
            Body = body;
            Created = created;
            CreatorUserID = creator.ID;
            CreatorIP = creatorIP;
            CreatorUserAgent = creatorUserAgent;
            Parent = parent;
            Post = post;
            Slug = slug;
            Language = language;
            State = state;

            userAuthenticated = creator;
        }

        public Comment(string body, DateTime created, Guid creatorUserID, string creatorName, string creatorEmail, string creatorEmailHash, string creatorUrl, long creatorIP, string creatorUserAgent, Guid id, Language language, DateTime modified, CommentSmall parent, PostSmall post, string slug, EntityState state)
            : this(id)
        {
            Body = body;
            Created = created;
            CreatorUserID = creatorUserID;
            CreatorName = creatorName;
            CreatorEmail = creatorEmail;
            CreatorEmailHash = creatorEmailHash;
            CreatorUrl = creatorUrl;
            CreatorIP = creatorIP;
            CreatorUserAgent = creatorUserAgent;
            Language = language;
            Modified = modified;
            Parent = parent;
            Post = post;
            Slug = slug;
            Language = language;
            State = state;
        }

        public PostSmall Post { get; private set; }
        [DataMember]
        public CommentSmall Parent { get; private set; }
        public Guid CreatorUserID { get; private set; }
        [DataMember]
        public string CreatorName { get; private set; }
        public string CreatorEmail { get; private set; }
        [DataMember]
        public string CreatorEmailHash { get; private set; }
        [DataMember]
        public string CreatorUrl { get; private set; }
        public long CreatorIP { get; private set; }
        public string CreatorUserAgent { get; private set; }
        public Language Language { get; private set; }
        [DataMember]
        public string Body { get; private set; }
        [DataMember]
        public string Slug { get; private set; }
        public EntityState State { get; private set; }
        [DataMember]
        public DateTime Created { get; private set; }
        public DateTime Modified { get; private set; }

        #region ICacheItem Members

        string ICacheItem.GetCacheItemKey()
        {
            return base.GetCacheItemKey();
        }

        IEnumerable<ICacheItem> ICacheItem.GetCacheDependencyItems()
        {
            return new EntityBase[] { new Post(Post.ID) };
        }

        #endregion

        private void setCreatorFields(UserAnonymous creator)
        {
            CreatorUserID = Guid.Empty;
            CreatorName = creator.Name;
            CreatorEmail = creator.Email;
            CreatorEmailHash = creator.EmailHash;
            CreatorUrl = creator.Url;
        }

        private void setupCreatorFields(UserAuthenticated creator)
        {
            CreatorUserID = creator.ID;
            CreatorName = !string.IsNullOrEmpty(creator.DisplayName) ? creator.DisplayName : creator.Name;
            CreatorEmail = creator.Email;
            CreatorEmailHash = creator.EmailHash;
            CreatorUrl = "";
        }
    }
}
