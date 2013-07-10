using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class Email
    {
        #region Private fields
        private List<EmailUser> _to = new List<EmailUser>();
        private List<EmailUser> _cc = new List<EmailUser>();
        private List<EmailUser> _bcc = new List<EmailUser>();
        private List<EmailAttachment> _attachments = new List<EmailAttachment>();
        private string _subject = string.Empty;
        private EmailUser _sender = null;
        private EmailBody _body = null;
        #endregion

        #region Constructor
        internal Email(EmailUser sender)
        {
            Sender = sender;
        }
        #endregion

        #region internal properties
        internal List<EmailUser> To
        {
            get
            {
                return _to;
            }
        }

        internal List<EmailUser> Cc
        {
            get
            {
                return _cc;
            }
            set
            {
                _cc = value;
            }
        }

        internal List<EmailUser> Bcc
        {
            get
            {
                return _bcc;
            }
        }

        internal List<EmailAttachment> Attachments
        {
            get
            {
                return _attachments;
            }
        }

        internal string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

        internal EmailBody Body
        {
            get { return _body; }
            set { _body = value; }
        }

        internal EmailUser Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
            }
        }

        internal Exception FailedByException { get; set; }
        #endregion

        #region Hidden methods
        private string EmailAddressesToString(List<EmailUser> emailUsers)
        {
            StringBuilder sb = new StringBuilder();
            emailUsers.ForEach(emailUserItem =>
            {
                if (sb.Length > 0)
                {
                    sb.Append(";");
                }
                sb.Append(emailUserItem.ToString());
            });
            return sb.ToString();
        }

        private void SetEmailAddresses(MailAddressCollection addressCollection, List<EmailUser> emailUsers)
        {
            emailUsers.ForEach(emailUserItem =>
            {
                MailAddress mailAddress = new MailAddress(emailUserItem.EmailAddress, emailUserItem.Name);
                addressCollection.Add(mailAddress);
            });
        }

        internal MailMessage BuildMail(ref string step)
        {
            MailMessage mail = null;
            step = "Validation";
            if (Sender == null)
            {
                throw new Exception("Sender must be set");
            }
            Sender.ValidateData();

            Attachments.ForEach(attachmentItem => attachmentItem.ValidateData());

            step = "Creating notify mail";
            mail = new MailMessage();

            string settingProperty = "Setting MailMessage '{0}' property: {1}";

            step = String.Format(settingProperty, "From", Sender.EmailAddress);
            if (!String.IsNullOrEmpty(Sender.Name))
            {
                mail.From = new MailAddress(Sender.EmailAddress, Sender.Name);
            }
            else
            {
                mail.From = new MailAddress(Sender.EmailAddress);
            }

            step = String.Format(settingProperty, "To", EmailAddressesToString(To));
            SetEmailAddresses(mail.To, To);

            step = String.Format(settingProperty, "CC", EmailAddressesToString(Cc));
            SetEmailAddresses(mail.CC, Cc);

            step = String.Format(settingProperty, "Bcc", EmailAddressesToString(Bcc));
            SetEmailAddresses(mail.Bcc, Bcc);

            step = String.Format(settingProperty, "Subject", Subject);
            mail.Subject = Subject;

            step = String.Format(settingProperty, "SubjectEncoding", System.Text.Encoding.UTF8);
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            if (Body != null)
            {
                step = String.Format(settingProperty, "IsBodyHtml & Body", Body.IsMessageHTML + " & <BODY>");
                Body.SetEmailBody(mail);
            }
            else
            {
                mail.IsBodyHtml = false;
            }

            step = String.Format(settingProperty, "Attachments", String.Join<EmailAttachment>(";", Attachments));
            Attachments.ForEach(attachementItem => mail.Attachments.Add(new Attachment(new MemoryStream(attachementItem.Content), attachementItem.Name)));

            return mail;
        }
        #endregion

        #region Metoder
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SENDER: " + Sender.ToString());
            if (To.Count > 0)
            {
                sb.AppendLine("TO: " + EmailAddressesToString(To));
            }
            if (Cc.Count > 0)
            {
                sb.AppendLine("CC: " + EmailAddressesToString(Cc));
            }
            if (Bcc.Count > 0)
            {
                sb.AppendLine("BCC: " + EmailAddressesToString(Bcc));
            }
            if (Subject != null)
            {
                sb.AppendLine("SUBJECT: " + Subject.ToString());
            }
            foreach (EmailAttachment attachment in Attachments)
            {
                sb.AppendLine("ATTACHMENT: " + attachment.ToString());
            }
            sb.AppendLine(Body.ToString());
            return sb.ToString();
        }
        #endregion
    }
}
