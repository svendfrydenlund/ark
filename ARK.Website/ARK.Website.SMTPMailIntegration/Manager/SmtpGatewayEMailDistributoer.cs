using ARK.Website.Common.DTO.EMail;
using ARK.Website.Common.Interface;
using ARK.Website.Common.Manager;
using ARK.Website.SMTPMailIntegration.Definitioner;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ARK.Website.SMTPMailIntegration.Manager
{
    public class SmtpGatewayEMailDistributoer : IEMailDistributoer
    {
        #region Private felter
        private const string MAIL_SENDING_SUCCES = "Mailforsendelse - Succes";
        private const string MAIL_SENDING_FAIL = "Mailforsendelse - Fejlet";

        private const string SETTINGS_SMTP_GATEWAY_FORBINDELSE = "SmtpGatewayForbindelse";
        private const string SETTINGS_SMTP_GATEWAY_TIMEOUT_I_MILLISEKUNDER = "SmtpGatewayTimeoutIMillisekunder";
        private string _smtpGatewayForbindelse = null;
        private int _smtpGatewayTimeoutIMillisekunder = 10000;
        #endregion

        #region Konstruktion
        public SmtpGatewayEMailDistributoer()
        {
            bool settingHarForening = ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem == SETTINGS_SMTP_GATEWAY_FORBINDELSE);
            bool settingHarForeningsnoegle = ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem == SETTINGS_SMTP_GATEWAY_TIMEOUT_I_MILLISEKUNDER);
            if ((!settingHarForening) || (!settingHarForeningsnoegle))
            {
                throw new Exception("App.Config eller Web.Config skal have noegler " + SETTINGS_SMTP_GATEWAY_FORBINDELSE + " og " + SETTINGS_SMTP_GATEWAY_TIMEOUT_I_MILLISEKUNDER + " med vaerdier");
            }
            _smtpGatewayForbindelse = ConfigurationManager.AppSettings[SETTINGS_SMTP_GATEWAY_FORBINDELSE];
            _smtpGatewayTimeoutIMillisekunder = Convert.ToInt32(ConfigurationManager.AppSettings[SETTINGS_SMTP_GATEWAY_TIMEOUT_I_MILLISEKUNDER]);
        }
        #endregion

        #region Private metoder
        private static Email Map(EMailHtmlForsendelseDTO request)
        {
            EmailUser sender = Map(request.Sender);
            Email mail = new Email(sender);
            mail.To.AddRange(Map(request.To));
            mail.Cc.AddRange(Map(request.Cc));
            mail.Bcc.AddRange(Map(request.Bcc));
            mail.Subject = request.Subject;
            mail.Body = Map(request.Body);
            mail.Attachments.AddRange(Map(request.Attachments));
            return mail;
        }

        private static EmailUser Map(EMailBrugerDTO emailUserDTO)
        {
            EmailUser emailUser = new EmailUser();
            if (emailUserDTO != null)
            {
                emailUser.EmailAddress = emailUserDTO.EMailAdresse;
                emailUser.Name = emailUserDTO.Navn;
            }
            return emailUser;
        }

        private static EmailBody Map(EMailHtmlBodyDTO emailBodyDTO)
        {
            EmailBody emailBody = new EmailBody();
            if (emailBodyDTO != null)
            {
                emailBody.IsMessageHTML = true;
                emailBody.Message = emailBodyDTO.BodyTekst;
                emailBody.HTMLEmbeddedImages.AddRange(Map(emailBodyDTO.IndlejretBilleder));
            }
            return emailBody;
        }

        private static EmailBodyHTMLEmbeddedImage Map(EMailHtmlBodyIndlejretBilledeDTO emailBodyEmbeddedImageDTO)
        {
            EmailBodyHTMLEmbeddedImage emailBodyEmbeddedImage = null;
            if (emailBodyEmbeddedImageDTO != null)
            {
                emailBodyEmbeddedImage = new EmailBodyHTMLEmbeddedImage();
                emailBodyEmbeddedImage.BodyImageSourceID = emailBodyEmbeddedImageDTO.BilledeID;
                emailBodyEmbeddedImage.Content = emailBodyEmbeddedImageDTO.Data;
            }
            return emailBodyEmbeddedImage;
        }

        private static EmailAttachment Map(EMailAttachmentDTO emailAttachmentDTO)
        {
            EmailAttachment emailAttachment = null;
            if (emailAttachmentDTO != null)
            {
                emailAttachment = new EmailAttachment();
                emailAttachment.Name = emailAttachmentDTO.Navn;
                emailAttachment.Content = emailAttachmentDTO.Data;
            }
            return emailAttachment;
        }

        private static List<EmailUser> Map(List<EMailBrugerDTO> emailUserDTOs)
        {
            List<EmailUser> emailUsers = new List<EmailUser>();
            if (emailUserDTOs != null)
            {
                foreach (EMailBrugerDTO emailUserDTO in emailUserDTOs)
                {
                    EmailUser emailUser = Map(emailUserDTO);
                    if (emailUser != null)
                    {
                        emailUsers.Add(emailUser);
                    }
                }
            }
            return emailUsers;
        }

        private static List<EmailBodyHTMLEmbeddedImage> Map(List<EMailHtmlBodyIndlejretBilledeDTO> emailBodyEmbeddedImageDTOs)
        {
            List<EmailBodyHTMLEmbeddedImage> emailBodyEmbeddedImages = new List<EmailBodyHTMLEmbeddedImage>();
            if(emailBodyEmbeddedImageDTOs != null)
            {
                foreach (EMailHtmlBodyIndlejretBilledeDTO emailBodyEmbeddedImageDTO in emailBodyEmbeddedImageDTOs)
                {
                    EmailBodyHTMLEmbeddedImage emailBodyEmbeddedImage = Map(emailBodyEmbeddedImageDTO);
                    if (emailBodyEmbeddedImage != null)
                    {
                        emailBodyEmbeddedImages.Add(emailBodyEmbeddedImage);
                    }
                }
            }
            return emailBodyEmbeddedImages;
        }

        private static List<EmailAttachment> Map(List<EMailAttachmentDTO> emailAttachmentDTOs)
        {
            List<EmailAttachment> emailAttachments = new List<EmailAttachment>();
            if (emailAttachmentDTOs != null)
            {
                foreach (EMailAttachmentDTO emailAttachmentDTO in emailAttachmentDTOs)
                {
                    EmailAttachment emailAttachment = Map(emailAttachmentDTO);
                    if (emailAttachment != null)
                    {
                        emailAttachments.Add(emailAttachment);
                    }
                }
            }
            return emailAttachments;
        }
        #endregion

        #region Metoder
        public void SendEMail(EMailHtmlForsendelseDTO eMailDefinition)
        {
            Email mail = Map(eMailDefinition);
            SmtpGateway smtpGateway = new SmtpGateway(_smtpGatewayForbindelse);
            smtpGateway.SmtpGatewayTimeoutInMiliseconds = _smtpGatewayTimeoutIMillisekunder;
            bool succeeded = smtpGateway.TrySend(mail);
            if (succeeded)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MAIL SENT");
                sb.AppendLine(mail.ToString());
                KomponentManager.LoggingManager.LogBesked(MAIL_SENDING_SUCCES, sb.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MAIL SENDING FAILED");
                sb.AppendLine(mail.ToString());
                sb.AppendLine("ERROR: " + mail.FailedByException.ToString());
                KomponentManager.LoggingManager.LogBesked(MAIL_SENDING_FAIL, sb.ToString());
            }
        }
        #endregion
    }
}
