// --------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://www.codeplex.com/oxite/license
// ---------------------------------
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Oxite.Modules.Blogs.Models;
using Oxite.Modules.Plugins.Extensions;
using Oxite.Modules.Plugins.Models;
using Oxite.Plugins.Attributes;
using Oxite.Services;

namespace OxiteSite.Plugins.Oxite.SpamControl
{
    [DisplayName("Spam Control")]
    [Description("Checks incomming comments against an Akismet API compatible service and marks comments as spam as approprate. Also allows comments to be explicitly marked as spam or ham from where the comment is displayed.")]
    [Authors("Nathan Heskew", "Erik Porter")]
    [AuthorUrls("http://nathan.heskew.com", "http://erikporter.com")]
    [IconLarge("ham_32x32.png")]
    [Category("AntiSpam")]
    [Tags("AntiSpam", "Akismet", "TypePad")]
    [Version(1, 0)]
    [ApiKeyValidation]
    public class SpamControlPlugin
    {
        public const string Version = "Oxite | SpamControl/1.0";
        public const string AkismetApiVersion = "1.1";

        [Required, Order(1), Group("API Settings", 1), Appearance(Width = "56%")]
        [LabelText("API key for the Akismet (http://akismet.com/personal/) or TypePad AntiSpam (http://antispam.typepad.com/info/get-api-key.html) service.")]
        public string ApiKey { get; set; }

        //[Required, Order(2), Group("API Settings", 1), Appearance(Width = "56%")]
        //[LabelText("API endpoint (currently Akismet == rest.akismet.com and TypePad AntiSpam == api.antispam.typepad.com)")]
        public string ApiEndpoint { get; set; }
        public object ApiEndpointDefinition 
        {
            get
            {
                return new
                {
                    LabelText = "API endpoint (currently Akismet == rest.akismet.com and TypePad AntiSpam == api.antispam.typepad.com)",
                    Required = true,
                    Order = 2,
                    Group = new PropertyGroup("API Settings", 1),
                    Appearance = new { Width = "56%" },
                    StringValidation = new
                    {
                        RegularExpressionMatcher = new Regex(@"^(?:(?:[a-z0-9!#$%&'*+\-/=?^_`{|}~]+\.)+[a-z]+|(?:\d{1,3}\.){3}\d{1,3})$")
                    }
                };
            }
        }

        public bool IsCommentSpam(ServiceContext context, PluginCommentInput comment)
        {
            SpamCandidate spamCandidate = new SpamCandidate
                {
                    blog = null,
                    comment_author = comment.CreatorName,
                    comment_author_email = comment.CreatorEmail,
                    comment_author_url = comment.CreatorUrl,
                    comment_content = comment.Body,
                    comment_type = "comment",
                    permalink = null,
                    referrer = context.HttpContext.Request.UrlReferrer,
                    user_agent = context.HttpContext.Request.UserAgent,
                    user_ip = context.HttpContext.Request.UserHostAddress
                };

            HttpWebRequest validationRequest = context.GeneratePostRequest(
                GetApiMethodUri("comment-check"),
                Version,
                spamCandidate.ToQueryString()
            );

            try
            {
                HttpWebResponse validationResponse = validationRequest.GetResponse() as HttpWebResponse;
                string responseCode = new StreamReader(validationResponse.GetResponseStream()).ReadToEnd();
                return bool.Parse(responseCode);
            }
            catch
            {
                return false;
            }
        }

        public void RegisterStyles(StyleList styles)
        {
            styles.Add("base.css", "ListForAdmin");
            styles.Add("base.css", "Item");
        }

        public void RegisterScripts(ScriptList scripts)
        {
            scripts.Add("base.js", "ListForAdmin");
            scripts.Add("base.js", "Item");
        }

        public void RegisterTemplates(TemplateList templates)
        {
            templates.Add("MarkAsSpam", "ul.comments li div.flags", SelectorType.AppendTo, "ListForAdmin", "Comment");
            templates.Add("MarkAsSpam", "ul.comments li div.flags", SelectorType.AppendTo, "Item", "Comment");
        }

        public void RegisterRoutes(RouteList routes)
        {
            routes.Add("MarkAsSpam", "Admin/Plugins/SpamControl/MarkAsSpam", new { role = "Admin" });
        }

        public void MarkAsSpam(RequestContext context, FormCollection form)
        {
            SpamCandidate spamCandidate = new SpamCandidate
            {
                blog = null,
                comment_author = form["commentCreatorName"],
                comment_author_email = form["commentCreatorEmail"],
                comment_author_url = form["commentCreatorUrl"],
                comment_content = form["commentBody"],
                comment_type = "comment",
                permalink = null,
                referrer = null,
                user_agent = form["commentCreatorUserAgent"],
                user_ip = form["commentCreatorIP"]
            };

            context.GeneratePostRequest(GetApiMethodUri("submit-spam"), Version, spamCandidate.ToQueryString()).GetResponse();
        }

        public void MarkAsHam(ServiceContext context, SpamCandidate spamCandidate)
        {
            context.GeneratePostRequest(GetApiMethodUri("submit-ham"), Version, spamCandidate.ToQueryString()).GetResponse();
        }

        public Uri GetApiKeyVerificationUri(string methodPath)
        {
            return new Uri(string.Format("http://{0}/{2}/{1}", ApiEndpoint, methodPath, AkismetApiVersion));
        }

        public Uri GetApiMethodUri(string methodPath)
        {
            return new Uri(string.Format("http://{0}.{1}/{3}/{2}", ApiKey, ApiEndpoint, methodPath, AkismetApiVersion));
        }
    }
}
