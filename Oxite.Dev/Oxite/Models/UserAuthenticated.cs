//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Oxite.Infrastructure;

namespace Oxite.Models
{
    [DataContract]
    public class UserAuthenticated : ICacheItem, IExtendedPropertyStore
    {
        private IEnumerable<Role> siteRoles;
        private IEnumerable<KeyValuePair<Guid, Role>> blogRoles;
        private IEnumerable<KeyValuePair<Guid, Role>> postRoles;
        private IEnumerable<KeyValuePair<Guid, Role>> pageRoles;

        public UserAuthenticated()
        {
            this.siteRoles = Enumerable.Empty<Role>();
            this.blogRoles = Enumerable.Empty<KeyValuePair<Guid, Role>>();
            this.postRoles = Enumerable.Empty<KeyValuePair<Guid, Role>>();
            this.pageRoles = Enumerable.Empty<KeyValuePair<Guid, Role>>();
        }

        public UserAuthenticated(Guid id, string userName, string displayName)
        {
            ID = id;
            Name = userName;
            DisplayName = displayName;
        }

        public UserAuthenticated(Guid id, string userName, string displayName, string email, string emailHash, Language language, EntityState status)
            : this(id, userName, displayName)
        {
            Email = email;
            EmailHash = emailHash;
            LanguageDefault = language;
            Status = status;
        }

        public UserAuthenticated(string userName, string displayName, string email, string emailHash, EntityState status)
        {
            Name = userName;
            DisplayName = displayName;
            Email = email;
            EmailHash = emailHash;
            Status = status;
        }

        public UserAuthenticated(string userName, string displayName, string email, string emailHash, Language language, EntityState status)
            : this(userName, displayName, email, emailHash, status)
        {
            LanguageDefault = language;
        }

        public UserAuthenticated(Guid id, string userName, string displayName, string email, string emailHash, EntityState status)
            : this(userName, displayName, email, emailHash, status)
        {
            ID = id;
        }

        public UserAuthenticated(Guid id, string userName, string displayName, string email, string emailHash, EntityState status, IEnumerable<Role> siteRoles, IEnumerable<KeyValuePair<Guid, Role>> blogRoles, IEnumerable<KeyValuePair<Guid, Role>> postRoles, IEnumerable<KeyValuePair<Guid, Role>> pageRoles)
            : this(userName, displayName, email, emailHash, status)
        {
            ID = id;
            this.siteRoles = siteRoles;
            this.blogRoles = blogRoles;
            this.postRoles = postRoles;
            this.pageRoles = pageRoles;
        }

        public Guid ID { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string DisplayName { get; private set; }
        [DataMember]
        public string Email { get; private set; }
        [DataMember]
        public string EmailHash { get; private set; }
        public EntityState Status { get; private set; }
        public Language LanguageDefault { get; private set; }
        public IEnumerable<Language> Languages
        {
            get { throw new NotImplementedException(); }
        }

        #region ICacheItem Members

        public string GetCacheItemKey()
        {
            return string.Format("User:{0:N}", ID);
        }

        public IEnumerable<ICacheItem> GetCacheDependencyItems()
        {
            return Enumerable.Empty<ICacheItem>();
        }

        #endregion

        #region IExtendedPropertyStore Members

        public IEnumerable<ExtendedProperty> ExtendedProperties { get; private set; }

        public string ScopeType
        {
            get { return this.GetType().FullName; }
        }

        public string ScopeKey
        {
            get { return ID.ToString("N"); }
        }

        #endregion

        public bool IsInRole(string roleName)
        {
            return isInRole(roleName, siteRoles);
        }

        public bool IsInRole(string roleName, Blog blog)
        {
            return IsInRole(roleName) || isInRole(roleName, blog.ID, blogRoles);
        }

        public bool IsInRole(string roleName, Post post)
        {
            return IsInRole(roleName) || IsInRole(roleName, post.Blog) || isInRole(roleName, post.ID, postRoles);
        }

        public bool IsInRole(string roleName, Page page)
        {
            return IsInRole(roleName) || isInRole(roleName, page.ID, pageRoles);
        }

        private static bool isInRole(string roleName, IEnumerable<Role> roles)
        {
            foreach (Role role in roles)
                if (string.Compare(role.Name, roleName, true) == 0)
                    return true;
                else if (role.Roles != null)
                    return isInRole(roleName, role.Roles);

            return false;
        }

        private static bool isInRole(string roleName, Guid id, IEnumerable<KeyValuePair<Guid, Role>> roles)
        {
            foreach (KeyValuePair<Guid, Role> role in roles)
                if (role.Key == id)
                    if (string.Compare(role.Value.Name, roleName, true) == 0)
                        return true;
                    else if (role.Value.Roles != null)
                        return isInRole(roleName, role.Value.Roles);

            return false;
        }
    }
}
