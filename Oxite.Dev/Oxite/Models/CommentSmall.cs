//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Runtime.Serialization;

namespace Oxite.Models
{
    [DataContract]
    public class CommentSmall
    {
        public CommentSmall(Guid id)
        {
            ID = id;
        }

        public CommentSmall(Guid id, DateTime created, UserAuthenticated creator, string slug)
            : this(id, created, slug)
        {
            CreatorName = !string.IsNullOrEmpty(creator.DisplayName) ? creator.DisplayName : creator.Name;
            CreatorEmailHash = creator.EmailHash;
            CreatorUrl = "";
        }

        public CommentSmall(Guid id, DateTime created, UserAnonymous creator, string slug)
            : this(id, created, slug)
        {
            CreatorName = creator.Name;
            CreatorEmailHash = creator.EmailHash;
            CreatorUrl = creator.Url;
        }

        private CommentSmall(Guid id, DateTime created, string slug)
            : this(id)
        {
            Created = created;
            Slug = slug;
        }

        [DataMember]
        public Guid ID { get; private set; }
        [DataMember]
        public DateTime Created { get; private set; }
        [DataMember]
        public string CreatorName { get; private set; }
        [DataMember]
        public string CreatorEmailHash { get; private set; }
        [DataMember]
        public string CreatorUrl { get; private set; }
        [DataMember]
        public string Slug { get; private set; }
    }
}
