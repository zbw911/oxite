using System;
using System.Xml.Serialization;
using System.Xml;

namespace BlogML.Xml
{
    [Serializable]
    public sealed class BlogMLContent
    {
        private bool base64 = false;
        private string text;

        [XmlAttribute("base64")]
        public bool Base64
        {
            get { return this.base64; }
            set { this.base64 = value; }
        }

        [XmlText]
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }

        [XmlIgnore]
        public string UncodedText
        {
            get
            {
                if (this.Base64)
                {
                    byte[] byteArray = Convert.FromBase64String(text);
                    return System.Text.Encoding.UTF8.GetString(byteArray);
                }
                else
                {
                    return this.Text;
                }
            }
        }

        public static BlogMLContent Create(string text, bool base64)
        {
            BlogMLContent content = new BlogMLContent();
            content.Base64 = base64;
            if (content.Base64)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(text);
                content.Text = Convert.ToBase64String(byteArray);
            }
            else
            {
                content.Text = text;
            }

            return content;
        }
    }
}
