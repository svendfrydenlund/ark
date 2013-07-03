using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dk.arok.data.conventus;
using dk.arok.data.conventus.Model;
using System.Collections.Generic;


namespace ConventusWrapper_Test
{



    [TestClass]
    public class ConventusWrapperUnitTest
    {

        private static string XML_START = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><conventus><medlemmer>";
        private static string XML_END = "</medlemmer></conventus>";

        private static string XML_ALL_FIELDS_DEFINED = "<medlem><id>1002282</id><alt_id>137</alt_id><type>person</type>      <navn>Svend Frydenlund</navn>      <adresse1>Gammelgang 10</adresse1>      <adresse2>Testhus</adresse2>      <postnr>2300</postnr>      <postnr_by>København S</postnr_by>      <tlf>35373691</tlf>      <mobil>22720203</mobil>      <email>svendfrydenlund@gmail.com</email>      <individuel1/>      <individuel2/>      <individuel3/>      <individuel4/>      <individuel5/>      <off_navn>true</off_navn>      <off_adresse1>true</off_adresse1>      <off_adresse2>true</off_adresse2>      <off_postnr>true</off_postnr>      <off_tlf>true</off_tlf>      <off_mobil>true</off_mobil>      <off_email>true</off_email>      <betalingskort_abonnement_transaktionsid/>      <betalingskort_abonnement_betalingsid>0</betalingskort_abonnement_betalingsid>      <betalingskort_abonnement_udloeb/>      <har_bs_aftale>false</har_bs_aftale>      <mangler_bekraeftigelse>false</mangler_bekraeftigelse>      <slettet>false</slettet> <birth>1974-02-14</birth>  <koen>mand</koen> </medlem>";
        private static string XML_ONLY_ID_DEFINED = "<medlem><id>1234</id><postnr>0</postnr><email>0</email><tlf>0</tlf></medlem>";
       

        [TestMethod]
        public void TestAllFieldsOneMember()
        {
            string xml = XML_START + XML_ALL_FIELDS_DEFINED + XML_END;

            Stream s = GenerateStreamFromString(xml);
            ConventusAddressWrapper caw = new ConventusAddressWrapper();
            List<ConventusMedlem> result = null;

            try
            {
                result = caw.getMembersFromXml(s);
            }
            catch (Exception e)
            {
                throw new AssertFailedException("Exception should not occur", e);
            }

            CollectionAssert.AllItemsAreNotNull(result);
            CollectionAssert.AllItemsAreUnique(result);
            Assert.IsTrue(result.Count == 1, "Only one result expected");

            ConventusMedlem m = result[0];

            Assert.AreEqual("Gammelgang 10", m.Adresse1);
            Assert.AreEqual("Testhus", m.Adresse2);
            Assert.AreEqual(137, m.AltID);
            Assert.AreEqual("svendfrydenlund@gmail.com", m.Email, "Email is wrong");
            Assert.AreEqual(new DateTime(1974, 2, 14), m.Foedselsdato);
            Assert.AreEqual(1002282, m.Id);
            Assert.AreEqual(ConventusMedlem.KoenTypes.Mand, m.Koen);
            Assert.AreEqual("22720203", m.Mobilnummer);
            Assert.AreEqual("Svend Frydenlund", m.Navn);
            Assert.IsTrue(m.OffentligAdresse, "offentlig adresse");
            Assert.IsTrue(m.OffentligEmail, "Offentlig email");
            Assert.IsTrue(m.OffentligTelefon, "Offentlig telefon");
            Assert.IsTrue(m.OfffentligNavn, "Offentlig navn");
            Assert.AreEqual("2300", m.Postnummer);
            Assert.AreEqual("København S", m.PostnummerBy);
            Assert.IsFalse(m.Slettet, "Slettet");
            Assert.AreEqual("35373691", m.Telefonnummer);

        }
        [TestMethod]
        public void TestXmlWithOnlyIdSet()
        { 
            string xml = XML_START + XML_ONLY_ID_DEFINED + XML_END;
            Stream s = GenerateStreamFromString(xml);
            
            ConventusAddressWrapper caw = new ConventusAddressWrapper();
            
            List<ConventusMedlem> result = null;
            
            try {result = caw.getMembersFromXml(s);
            } catch (Exception e) {
                throw new AssertFailedException("Exception should not occur", e);
            }

            CollectionAssert.AllItemsAreNotNull(result);
            CollectionAssert.AllItemsAreUnique(result);
            Assert.IsTrue(result.Count == 1, "Only one result expected");
            Assert.AreEqual(1234, result[0].Id, "id not as expected");
        }

        [TestMethod]
        public void TestPostnummerEmailTelefonWithZeroValues()
        {
            string xml = XML_START + "<medlem><id>1234</id><postnr>0</postnr><email>0</email><tlf>0</tlf></medlem>" + XML_END;
            Stream s = GenerateStreamFromString(xml);

            ConventusAddressWrapper caw = new ConventusAddressWrapper();

            List<ConventusMedlem> result = null;

            try
            {
                result = caw.getMembersFromXml(s);
            }
            catch (Exception e)
            {
                throw new AssertFailedException("Exception should not occur", e);
            }

            CollectionAssert.AllItemsAreNotNull(result);
            CollectionAssert.AllItemsAreUnique(result);
            Assert.IsTrue(result.Count == 1, "Only one result expected");

            Assert.AreEqual(string.Empty, result[0].Postnummer, "Postnummer should be empty");
            Assert.AreEqual(string.Empty, result[0].Email, "Email should be empty");
            Assert.AreEqual(string.Empty, result[0].Telefonnummer, "Telefonnummer should be empty");
        }

        [TestMethod]
        public void TestPublicAddress()
        {
            Assert.Fail("Test not implemented yet");
        }

        [TestMethod]
        public void TestMultipleMembers()
        {
            string xml = XML_START + XML_ALL_FIELDS_DEFINED + XML_ONLY_ID_DEFINED + XML_END;
            Stream s = GenerateStreamFromString(xml);
            ConventusAddressWrapper caw = new ConventusAddressWrapper();
            List<ConventusMedlem> result = null;

            try
            {
                result = caw.getMembersFromXml(s);
            }
            catch (Exception e)
            {
                throw new AssertFailedException("Exception should not occur", e);
            }

            CollectionAssert.AllItemsAreNotNull(result);
            CollectionAssert.AllItemsAreUnique(result);
            Assert.IsTrue(result.Count == 2, "2 results expected");

        }

        [TestMethod]
        public void TestErrorInResponse()
        {
            // Create non complete xml.
            string xml = XML_START;
            Stream s = GenerateStreamFromString(xml);
            ConventusAddressWrapper caw = new ConventusAddressWrapper();
            List<ConventusMedlem> result = null;

            Boolean xmlexceptionOccured = false;

            try
            {
                result = caw.getMembersFromXml(s);
            }
            catch (System.Xml.XmlException)
            {
                // Expected
                xmlexceptionOccured = true;
                
                
            }

            if (!xmlexceptionOccured)
            {
                Assert.Fail("XMLException should have occured");
            }
        }


        /// <summary>
        /// Helper method to create a stream from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
     

    }
}
