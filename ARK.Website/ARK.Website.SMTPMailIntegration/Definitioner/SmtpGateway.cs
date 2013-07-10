using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class SmtpGateway
    {
        #region Fields
        private string _smtpGatewayEndpoint = null;
        private int _smtpGatewayTimeoutInMiliseconds = 100000;
        #endregion

        #region Constructor
        internal SmtpGateway(string smtpGatewayEndpoint)
        {
            SmtpGatewayEndpoint = smtpGatewayEndpoint;
        }
        #endregion

        #region Properties
        internal string SmtpGatewayEndpoint
        {
            get
            {
                return _smtpGatewayEndpoint;
            }
            set
            {
                _smtpGatewayEndpoint = value;
            }
        }

        internal int SmtpGatewayTimeoutInMiliseconds
        {
            get
            {
                return _smtpGatewayTimeoutInMiliseconds;
            }
            set
            {
                _smtpGatewayTimeoutInMiliseconds = value;
            }
        }
        #endregion

        #region Hidden methods
        private void SendMail(MailMessage mail, ref string step, ref string possibleFailReason)
        {
            SmtpClient client = null;
            try
            {
                if (String.IsNullOrEmpty(SmtpGatewayEndpoint))
                {
                    throw new Exception("SmtpGatewayEndpoint missing");
                }

                string settingProperty = "Setting SmtpClient '{0}' property: {1}";
                step = "Creating SmtpClient with endpoint: " + SmtpGatewayEndpoint;
                client = new SmtpClient(SmtpGatewayEndpoint);

                // Add credentials if the SMTP server requires them.
                // Credentials are necessary if the server requires the client 
                // to authenticate before it will send e-mail on the client's behalf.
                step = String.Format(settingProperty, "Credentials", "CredentialCache.DefaultNetworkCredentials");
                client.Credentials = CredentialCache.DefaultNetworkCredentials;

                //Specifies the time-out value in milliseconds. The default value is 100,000 (100 seconds).
                step = String.Format(settingProperty, "Timeout", SmtpGatewayTimeoutInMiliseconds);
                client.Timeout = SmtpGatewayTimeoutInMiliseconds;

                try
                {
                    //Send the message.
                    step = "Sending MailMessage through SmtpClient";
                    client.Send(mail);
                }
                catch (ArgumentNullException ane)
                {
                    possibleFailReason = "Message is undefined(null) or FROM property is undefined";
                    throw ane;
                }
                catch (ObjectDisposedException ode)
                {
                    possibleFailReason = "The SmtpClient has been disposed";
                    throw ode;
                }
                catch (InvalidOperationException ioe)
                {
                    possibleFailReason =
                        " - This SmtpClient has a SendAsync call in progress." + Environment.NewLine +
                        " - There are no recipients specified in T0, CC, Bcc properties." + Environment.NewLine +
                        " - DeliveryMethod property is set to Network and Host is undefined(null or empty string)" + Environment.NewLine +
                        " - Or Port is 0 (zero), a negative number, or greater than 65,535.";
                    throw ioe;
                }
                catch (SmtpFailedRecipientsException sfre)
                {
                    possibleFailReason = "The message could not be delivered to one or more of the recipients in To, CC or Bcc";
                    throw sfre;
                }
                catch (SmtpException se)
                {
                    possibleFailReason =
                        " - The connection to the SMTP server failed." + Environment.NewLine +
                        " - Authentication failed." + Environment.NewLine +
                        " - The operation timed out." + Environment.NewLine +
                        " - EnableSsl is set to true but:" + Environment.NewLine +
                        "   * The DeliveryMethod property is set to SpecifiedPickupDirectory or PickupDirectoryFromIis." + Environment.NewLine +
                        "   * The SMTP mail server did not advertise STARTTLS in the response to the EHLO command." + Environment.NewLine;

                    throw se;
                }
                catch (Exception ex)
                {
                    possibleFailReason = "";
                    throw ex;
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }
        #endregion

        #region Methods
        internal bool TrySend(Email mail)
        {
            string step = "";
            Exception failedByException = null;
            string possibleFailReason = "";

            try
            {
                if (mail == null)
                {
                    throw new ArgumentNullException("Input parameter: mail");
                }

                step = "Building MailMessage";
                MailMessage localMail = mail.BuildMail(ref step);

                step = "Building SmtpGateway and sending mail";
                SendMail(localMail, ref step, ref possibleFailReason);
            }
            catch (Exception exception)
            {
                failedByException = exception;
            }

            if (failedByException != null)
            {
                string errorMessage = "## FAILED ##";
                if (!String.IsNullOrEmpty(step))
                {
                    errorMessage += Environment.NewLine + "Step: " + Environment.NewLine + step;
                }

                if (!String.IsNullOrEmpty(possibleFailReason))
                {
                    errorMessage += Environment.NewLine + "Possible Reason: " + Environment.NewLine + possibleFailReason;
                }
                mail.FailedByException = new Exception(errorMessage, failedByException);
            }

            return failedByException == null;
        }
        #endregion
    }
}
