//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using Oxite.Models;

namespace Oxite.ViewModels
{
    public class UserViewModel
    {
        private readonly UserAnonymous userAnonymous;
        private readonly UserAuthenticated userAuthenticated;

        public UserViewModel(UserAnonymous user)
        {
            userAnonymous = user;

            if (user != null)
            {
                Name = DisplayName = user.Name;
                Email = user.Email;
                EmailHash = user.EmailHash;
                Url = user.Url;
            }
        }

        public UserViewModel(UserAuthenticated user)
        {
            userAuthenticated = user;

            Name = user.Name;
            DisplayName = user.DisplayName;
            Email = user.Email;
            EmailHash = user.EmailHash;
            Url = "";
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Email { get; private set; }
        public string EmailHash { get; private set; }
        public string Url { get; private set; }
        public bool IsAuthenticated { get { return userAuthenticated != null; } }

        public bool IsInRole(string roleName)
        {
            if (userAuthenticated == null) return false;

            return userAuthenticated.IsInRole(roleName);
        }

        public bool IsInRole(string roleName, Blog blog)
        {
            if (userAuthenticated == null) return false;

            return userAuthenticated.IsInRole(roleName, blog);
        }

        public bool IsInRole(string roleName, Post post)
        {
            if (userAuthenticated == null) return false;

            return userAuthenticated.IsInRole(roleName, post);
        }

        public bool IsInRole(string roleName, Page page)
        {
            if (userAuthenticated == null) return false;

            return userAuthenticated.IsInRole(roleName, page);
        }

        public UserAnonymous ToUserAnonymous()
        {
            return userAnonymous;
        }

        public UserAuthenticated ToUserAuthenticated()
        {
            return userAuthenticated;
        }
    }
}
