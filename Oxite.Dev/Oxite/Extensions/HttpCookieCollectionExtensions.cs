//  --------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  This source code is made available under the terms of the Microsoft Public License (Ms-PL)
//  http://www.codeplex.com/oxite/license
//  ---------------------------------
using System;
using System.Web;
using Oxite.Models;
using Oxite.Models.Extensions;

namespace Oxite.Extensions
{
    public static class HttpCookieCollectionExtensions
    {
        private const string anonymousUserCookieName = "anon";

        public static void ClearAnonymousUser(this HttpCookieCollection cookies)
        {
            cookies.Add(new HttpCookie(anonymousUserCookieName) { Expires = DateTime.Now.AddDays(-1) });
        }

        public static UserAnonymous GetAnonymousUser(this HttpCookieCollection cookies)
        {
            HttpCookie cookie = cookies[anonymousUserCookieName];
            UserAnonymous user = null;

            if (cookie != null)
            {
                try
                {
                    user = typeof(UserAnonymous).FromJson(cookie.Value) as UserAnonymous;
                }
                catch { }
            }

            return user;
        }

        public static void SetAnonymousUser(this HttpCookieCollection cookies, UserAnonymous user)
        {
            HttpCookie cookie = new HttpCookie(anonymousUserCookieName, user.ToJson());

            cookie.Expires = DateTime.Now.AddDays(14);

            cookies.Add(cookie);
        }

        public static string GetSkinName(this HttpCookieCollection cookies)
        {
            string skin = null;
            HttpCookie cookie = cookies["skin"];

            if (cookie != null)
                skin = cookie.Value;

            return skin;
        }
    }
}
