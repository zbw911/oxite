using System;
using System.Xml;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;

namespace BlogML
{
    public abstract class BlogMLWriterBase
    {

        private XmlWriter writer;

        protected XmlWriter Writer
        {
            get { return this.writer; }
        }

        public void Write(XmlWriter writer)
        {
            this.writer = writer;

            //Write the XML delcaration. 
            writer.WriteStartDocument();

            try
            {
                this.InternalWriteBlog();
            }
            finally
            {
                this.writer = null;
            }
        }


        protected abstract void InternalWriteBlog();

        #region WriteStartBlog


        protected void WriteStartBlog(string title, string subTitle, string rootUrl)
        {
            this.WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, DateTime.Now);
        }

        protected void WriteStartBlog(string title, string subTitle, string rootUrl, DateTime dateCreated)
        {
            this.WriteStartBlog(title, ContentTypes.Text, subTitle, ContentTypes.Text, rootUrl, dateCreated);
        }


        protected void WriteStartBlog(
            string title,
            ContentTypes titleContentType,
            string subTitle,
            ContentTypes subTitleContentType,
            string rootUrl,
            DateTime dateCreated)
        {

            this.WriteStartElement("blog");
            this.WriteAttributeStringRequired("root-url", rootUrl);
            this.WriteAttributeString("date-created", FormatDateTime(dateCreated));

            // Write the default namespace, identified as xmlns with no prefix
            writer.WriteAttributeString("xmlns", null, null, "http://www.blogml.com/2006/09/BlogML");
            writer.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");

            this.WriteContent2("title", title, titleContentType);
            this.WriteContent2("sub-title", subTitle, subTitleContentType);
        }

        #endregion

        protected void WriteStartElement(string tag)
        {
            this.Writer.WriteStartElement(tag);
        }


        protected void WriteEndElement()
        {
            this.Writer.WriteEndElement();
        }


        #region Extended Properties

        protected void WriteStartExtendedProperties()
        {
            this.WriteStartElement("extended-properties");
        }

        protected void WriteExtendedProperty(string Name, string Value)
        {
            this.WriteStartElement("property");
            this.WriteAttributeString("name", Name);
            this.WriteAttributeString("value", Value);
            this.WriteEndElement();
        }

        #endregion


        protected void WriteAuthor(
            string id,
            string name,
            string email,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved
            )
        {
            this.WriteStartElement("author");
            this.WriteNodeAttributes(id, dateCreated, dateModified, approved);
            this.WriteAttributeString("email", email);
            this.WriteContent2("title", name, ContentTypes.Text);
            this.WriteEndElement();
        }


        protected void WriteCategory(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string description,
            string parentRef
            )
        {
            this.WriteCategory(id, title, ContentTypes.Text, dateCreated, dateModified, approved, description, parentRef);
        }


        protected void WriteCategory(
            string id,
            string title,
            ContentTypes titleContentType,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string description,
            string parentRef
            )
        {
            this.WriteStartElement("category");
            this.WriteNodeAttributes(id, dateCreated, dateModified, approved);
            this.WriteAttributeString("description", description);
            this.WriteAttributeString("parentref", parentRef);
            this.WriteContent2("title", title, titleContentType);
            this.WriteEndElement();
        }

        protected void WriteAuthorReference(
            string refID
            )
        {
            this.WriteStartElement("author");
            this.WriteAttributeStringRequired("ref", refID);
            this.WriteEndElement();
        }

        protected void WriteCategoryReference(
            string refID
            )
        {
            this.WriteStartElement("category");
            this.WriteAttributeStringRequired("ref", refID);
            this.WriteEndElement();
        }

        protected void WriteStartAuthors()
        {
            this.WriteStartElement("authors");
        }

        protected void WriteStartCategories()
        {
            this.WriteStartElement("categories");
        }


        protected void WriteStartPosts()
        {
            this.WriteStartElement("posts");
        }


        protected void WriteStartTrackbacks()
        {
            this.WriteStartElement("trackbacks");
        }


        protected void WriteStartAttachments()
        {
            this.WriteStartElement("attachments");
        }


        protected void WriteNodeAttributes(
            string id,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved
            )
        {
            this.WriteAttributeString("id", id);
            this.WriteAttributeString("date-created", FormatDateTime(dateCreated));
            this.WriteAttributeString("date-modified", FormatDateTime(dateModified));
            this.WriteAttributeString("approved", approved ? "true" : "false");
        }


        protected string FormatDateTime(DateTime date)
        {
            return date.ToUniversalTime().ToString("s");
        }


        protected void WriteStartPost(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string content,
            string postUrl
            )
        {
            this.WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text, postUrl
                , 0, false, null, ContentTypes.Text, BlogPostTypes.Normal, string.Empty);
        }


        protected void WriteStartPost(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string content,
            string postUrl,
            UInt32 Views,
            BlogPostTypes blogpostType,
            string PostName
            )
        {
            this.WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text, postUrl
                , Views, false, null, ContentTypes.Text, blogpostType, PostName);
        }

        protected void WriteStartPost(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string content,
            string postUrl,
            UInt32 Views,
            string excerpt,
            BlogPostTypes blogpostType,
            string PostName
            )
        {
            this.WriteStartPost(id, title, ContentTypes.Text, dateCreated, dateModified, approved, content, ContentTypes.Text,
                postUrl, Views, true, excerpt, ContentTypes.Text, blogpostType, PostName);
        }


        protected void WriteStartPost(
            string id,
            string title,
            ContentTypes titleContentType,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string content,
            ContentTypes postContentType,
            string postUrl,
            UInt32 Views,
            bool hasexcerpt,
            string excerpt,
            ContentTypes excerptContentType,
            BlogPostTypes blogpostType,
            string PostName
            )
        {
            this.WriteStartElement("post");
            this.WriteNodeAttributes(id, dateCreated, dateModified, approved);
            this.WriteAttributeString("post-url", postUrl);
            this.WriteAttributeStringRequired("type", blogpostType.ToString().ToLower());
            this.WriteAttributeStringRequired("hasexcerpt", hasexcerpt.ToString().ToLower());
            this.WriteAttributeStringRequired("views", Views.ToString());
            this.WriteContent2("title", title, titleContentType);
            this.WriteContent2("content", content, postContentType);           
            if (PostName != null)
                this.WriteContent2("post-name", PostName, ContentTypes.Text);
            if (hasexcerpt == true)
                this.WriteContent2("excerpt", excerpt, excerptContentType);
        }


        protected void WriteStartComments()
        {
            this.WriteStartElement("comments");
        }


        protected void WriteComment(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string userName,
            string userEmail,
            string userUrl,
            string content
            )
        {
            this.WriteComment(id, title, ContentTypes.Text, dateCreated, dateModified, approved, userName, userEmail, userUrl, content, ContentTypes.Text);
        }


        protected void WriteComment(
            string id,
            string title,
            ContentTypes titleContentType,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string userName,
            string userEmail,
            string userUrl,
            string content,
            ContentTypes commentContentType
            )
        {
            this.WriteStartElement("comment");
            this.WriteNodeAttributes(id, dateCreated, dateModified, approved);
            this.WriteAttributeStringRequired("user-name", (userName == null) ? "" : userName);
            this.WriteAttributeString("user-url", (userUrl == null) ? "" : userUrl);
            this.WriteAttributeString("user-email", (userEmail == null) ? "" : userEmail);
            this.WriteContent2("title", title, titleContentType);
            this.WriteContent2("content", content, commentContentType);
            this.WriteEndElement();
        }


        protected void WriteTrackback(
            string id,
            string title,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string url
            )
        {
            this.WriteTrackback(id, title, ContentTypes.Text, dateCreated, dateModified, approved, url);
        }


        protected void WriteTrackback(
            string id,
            string title,
            ContentTypes titleContentType,
            DateTime dateCreated,
            DateTime dateModified,
            bool approved,
            string url
            )
        {
            this.WriteStartElement("trackback");
            this.WriteNodeAttributes(id, dateCreated, dateModified, approved);
            this.WriteAttributeStringRequired("url", url);
            this.WriteContent2("title", title, titleContentType);
            this.WriteEndElement();
        }


        protected void WriteAttributeStringRequired(string name, string value)
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException("value", name);
            this.Writer.WriteAttributeString(name, value);
        }


        protected void WriteAttributeString(string name, string value)
        {
            if (value != null && value.Length > 0)
                this.Writer.WriteAttributeString(name, value);
        }


        [Obsolete("Use WriteContent2", true)]
        protected void WriteContent(
            string text,
            bool base64)
        {
            this.WriteStartElement("content");
            this.WriteAttributeString("base64", base64 ? "true" : "false");
            if (base64)
            {
                System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();

                int byteCount = ascii.GetByteCount(text);
                Byte[] bytes = new Byte[byteCount];
                int bytesEncodedCount = ascii.GetBytes(text, 0, text.Length, bytes, 0);

                // byte[] bytes = writer.Settings.Encoding.GetBytes(text);
                this.Writer.WriteBase64(bytes, 0, bytes.Length);
            }
            else
                this.Writer.WriteString(text);
            this.WriteEndElement();
        }


        protected void WriteContent2(
            string elementName,
            string text,
            ContentTypes contentType)
        {

            this.WriteStartElement(elementName);
            this.WriteAttributeString("type", contentType.ToString().ToLower());

            if (contentType == ContentTypes.Base64)
            {
                System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();

                int byteCount = ascii.GetByteCount(text);
                Byte[] bytes = new Byte[byteCount];
                text = Convert.ToBase64String(bytes);
            }

            this.Writer.WriteCData(text);
            this.WriteEndElement();
        }

        protected void WriteAttachment(string externalUri, string mimeType, string fullUrl)
        {           
            this.WriteAttachment(fullUrl, 0, mimeType, externalUri, false, null);
        }

        protected void WriteAttachment(
            string embeddedUrl,
            string mimeType,
            Stream inputStream
            )
        {
            using (BinaryReader reader = new BinaryReader(inputStream))
            {
                reader.BaseStream.Position = 0;
                byte[] data = reader.ReadBytes((int)inputStream.Length);
                WriteAttachment(embeddedUrl, data.Length, mimeType, null, true, data);
            }
        }

        protected void WriteAttachment(
            string embeddedUrl,
            double size,
            string mimeType,
            string externalUri,
            bool embedded,
            byte[] data
            )
        {
            this.WriteStartElement("attachment");

            try
            {

                this.WriteAttributeStringRequired("url", embeddedUrl);

                if (size > 0)
                    this.WriteAttributeStringRequired("size", size.ToString());

                if (mimeType != null)
                    this.WriteAttributeStringRequired("mime-type", mimeType);

                if (externalUri != null && externalUri.Length > 0)
                    this.WriteAttributeStringRequired("external-uri", externalUri);

                this.WriteAttributeString("embedded", embedded ? "true" : "false");

                if (embedded)
                {
                    this.Writer.WriteBase64(data, 0, data.Length);
                }
            }
            finally
            {
                this.WriteEndElement();
            }

        }


        internal void CopyStream(Stream src, Stream dst)
        {
            byte[] buf = new byte[4096];
            while (true)
            {
                int bytesRead = src.Read(buf, 0, buf.Length);

                //Read returns 0 when reached end of stream.
                if (bytesRead == 0)
                    break;
                else
                    dst.Write(buf, 0, bytesRead);
            }
        }


        public sealed class SgmlUtil
        {
            public static bool IsRootUrlOf(string rootUrl, string url)
            {

                if (rootUrl == null)
                {
                    throw new ArgumentNullException("rootUrl");
                }

                if (url == null)
                {
                    throw new ArgumentNullException("url");
                }

                rootUrl = rootUrl.Trim().ToLower();
                url = url.Trim().ToLower();
                // is it a full path
                if (url.StartsWith("http://"))
                    return url.StartsWith(rootUrl);
                else
                    // it's local
                    return true;
            }

            public static string StripRootUrlPath(string rootUrl, string url)
            {
                if (url.StartsWith(rootUrl))
                {
                    url = url.Remove(0, rootUrl.Length);
                }

                if (url.StartsWith("/"))
                {
                    url.TrimStart(new char[] { '/' });
                }

                return url;
            }

            public static string CleanAttachmentUrls(string content, string oldPath, string newPath)
            {

                oldPath = Regex.Escape(oldPath);

                MatchCollection matches = Regex.Matches(content, oldPath);
                content = Regex.Replace(content, oldPath, newPath,
                    RegexOptions.CultureInvariant
                    | RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.ExplicitCapture);
                return content;
            }

            public static string[] GetAttributeValues(string content, string tag, string attribute)
            {
                Regex srcrx = CreateAttributeRegex(attribute);

                MatchCollection matches = CreateTagRegex(tag).Matches(content);
                string[] sources = new string[matches.Count];
                for (int i = 0; i < sources.Length; ++i)
                {

                    Match m = srcrx.Match(matches[i].Value);
                    sources[i] = m.Groups["Value"].Value;
                }
                return sources;
            }

            public static Regex CreateTagRegex(string name)
            {

                string pattern = @"<\s*{0}[^>]+>";

                pattern = string.Format(pattern, name);
                return new Regex(
                    pattern,
                    RegexOptions.CultureInvariant
                    | RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.ExplicitCapture
                    );
            }

            public static Regex CreateAttributeRegex(string name)
            {
                string pattern = @"{0}\s*=\s*['""]?\s*(?<Value>[^'"" ]+)";
                pattern = string.Format(pattern, name);
                return new Regex(
                    pattern,
                    RegexOptions.CultureInvariant
                    | RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.ExplicitCapture
                    );
            }

        }

    }
}

