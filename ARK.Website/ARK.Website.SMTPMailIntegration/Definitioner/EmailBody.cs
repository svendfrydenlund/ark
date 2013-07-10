using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class EmailBody
    {
        #region Fields
        private string _message = string.Empty;
        private bool _isMessageHTML = false;
        private List<EmailBodyHTMLEmbeddedImage> _htmlEmbeddedImages = new List<EmailBodyHTMLEmbeddedImage>();
        #endregion

        #region Properties
        internal string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        internal bool IsMessageHTML
        {
            get
            {
                return _isMessageHTML;
            }
            set
            {
                _isMessageHTML = value;
            }
        }

        internal List<EmailBodyHTMLEmbeddedImage> HTMLEmbeddedImages
        {
            get
            {
                return _htmlEmbeddedImages;
            }
        }
        #endregion

        #region Methods
        internal void ValidateData()
        {
            if (!String.IsNullOrEmpty(Message))
            {
                foreach (EmailBodyHTMLEmbeddedImage embeddedImageItem in HTMLEmbeddedImages)
                {
                    embeddedImageItem.ValidateData();
                    if (!Message.Contains(embeddedImageItem.BodyImageSourceID))
                    {
                        throw new Exception(typeof(EmailBody).Name + ": BodyImageSourceID " + "[" + embeddedImageItem.BodyImageSourceID + "] not found in EmailBody.Message");
                    }
                }
            }
        }

        internal void SetEmailBody(MailMessage mail)
        {
            if (!String.IsNullOrEmpty(Message))
            {
                mail.IsBodyHtml = IsMessageHTML;

                if (IsMessageHTML)
                {
                    List<LinkedResource> linkedResources = new List<LinkedResource>();
                    Dictionary<string, string> replacements = new Dictionary<string, string>();

                    foreach (EmailBodyHTMLEmbeddedImage embeddedImageItem in HTMLEmbeddedImages)
                    {
                        LinkedResource linkedResource = new LinkedResource(new MemoryStream(embeddedImageItem.Content));
                        linkedResource.ContentId = Guid.NewGuid().ToString();
                        replacements.Add(embeddedImageItem.BodyImageSourceID, "cid:" + linkedResource.ContentId);
                        linkedResources.Add(linkedResource);
                    }

                    string convertedBody = MultipleReplace(Message, replacements);
                    AlternateView view = AlternateView.CreateAlternateViewFromString(convertedBody, null, "text/html");
                    linkedResources.ForEach(linkedResourceItem => view.LinkedResources.Add(linkedResourceItem));
                    mail.AlternateViews.Add(view);
                }
                else
                {
                    mail.Body = Message;
                }
            }
        }

        private string MultipleReplace(string text, Dictionary<string, string> replacements)
        {
            string returnText = text;
            if (replacements != null && replacements.Count != 0)
            {
                returnText = Regex.Replace(text, "(" + String.Join("|", replacements.Keys.ToArray()) + ")", delegate(Match m) { return replacements[m.Value]; });
            }
            return returnText;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Message != null)
            {
                sb.AppendLine("E-MAIL BODY: " + Message);
            }
            else
            {
                sb.AppendLine("E-MAIL BODY: NO BODY");
            }
            foreach (EmailBodyHTMLEmbeddedImage image in HTMLEmbeddedImages)
            {
                sb.AppendLine("E-MAIL BODY - EMBEDDED IMAGE: " + image.ToString());
            }
            return sb.ToString();
        }
        #endregion
    }
}
