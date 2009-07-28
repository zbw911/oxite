//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System.Runtime.Serialization;

namespace Oxite.Models
{
    [DataContract]
    public class UserAnonymous
    {
        public UserAnonymous(string name, string email, string emailHash, string url)
        {
            Name = name;
            Email = email;
            EmailHash = emailHash;
            Url = url;
        }

        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Email { get; private set; }
        [DataMember]
        public string EmailHash { get; private set; }
        [DataMember]
        public string Url { get; private set; }
    }
}
