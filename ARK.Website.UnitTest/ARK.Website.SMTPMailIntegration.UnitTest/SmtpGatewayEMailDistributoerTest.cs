using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.UnitTest.Common;
using ARK.Website.Common.Manager;
using ARK.Website.SMTPMailIntegration.Manager;
using ARK.Website.Common.DTO.EMail;
using System.IO;

namespace ARK.Website.SMTPMailIntegration.UnitTest
{
    [TestClass]
    public class SmtpGatewayEMailDistributoerTest
    {
        private EMailHtmlBodyIndlejretBilledeDTO HentIndlejretBillede(string path)
        {
            EMailHtmlBodyIndlejretBilledeDTO billede = null;
            if (File.Exists(path))
            {
                billede = new EMailHtmlBodyIndlejretBilledeDTO();
                billede.BilledeID = Guid.NewGuid().ToString();
                billede.Data = File.ReadAllBytes(path);
            }
            return billede;
        }

        private EMailAttachmentDTO HentAttachmentBillede(string path)
        {
            EMailAttachmentDTO billede = null;
            if (File.Exists(path))
            {
                billede = new EMailAttachmentDTO();
                billede.Navn = Path.GetFileName(path);
                billede.Data = File.ReadAllBytes(path);
            }
            return billede;
        }

        [TestMethod]
        public void SendEMailTest()
        {
            UnitTestHelper.InitierAlleKomponenterMedDefault();
            SmtpGatewayEMailDistributoer eMailDistributoer = new SmtpGatewayEMailDistributoer();
            LoggingManagerUnitTest loggingManager = new LoggingManagerUnitTest();
            KomponentManager.EMailDistributoer = eMailDistributoer;
            KomponentManager.LoggingManager = loggingManager;

            EMailHtmlBodyIndlejretBilledeDTO indlejretBillede = HentIndlejretBillede(Directory.GetCurrentDirectory() + @"\Billeder\smileyOne.jpg");
            EMailAttachmentDTO attachmentBillede = HentAttachmentBillede(Directory.GetCurrentDirectory() + @"\Billeder\smileyTwo.jpg");
            EMailBrugerDTO brugerThomasDalsgaard = new EMailBrugerDTO();
            brugerThomasDalsgaard.EMailAdresse = "tasd@camite.com";
            brugerThomasDalsgaard.Navn = "Thomas Dalsgaard";
            string subject = "Smil!!!";
            EMailHtmlBodyDTO body = new EMailHtmlBodyDTO();
            body.IndlejretBilleder.Add(indlejretBillede);
            body.BodyTekst =
                "<html><body>" +
                "<h1>Smil og vær glad ;)</h1>" +
                "<p>Vær ikke genert!</p>" +
                "<img src=\"" + indlejretBillede.BilledeID +"\" alt=\"Smiley face\" height=\"42\" width=\"42\">" +
                "<p>Se det er ikke så svært ^^</p>" +
                "</body></html>";

            EMailHtmlForsendelseDTO forsendelse = new EMailHtmlForsendelseDTO();
            forsendelse.Sender = brugerThomasDalsgaard;
            forsendelse.To.Add(brugerThomasDalsgaard);
            forsendelse.Subject = subject;
            forsendelse.Body = body;
            forsendelse.Attachments.Add(attachmentBillede);

            bool succes = false;
            try
            {
                eMailDistributoer.SendEMail(forsendelse);
                succes = true;
            }
            catch (Exception)
            {

            }
            string resultat = loggingManager.ToString();
            Assert.IsTrue(succes);
        }
    }
}
