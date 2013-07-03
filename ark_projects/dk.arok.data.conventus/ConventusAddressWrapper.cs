using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using NLog;



using dk.arok.data.conventus.Model;

/// todo 2: Logging framework


namespace dk.arok.data.conventus
{
    public class ConventusAddressWrapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        
        private static string ADRESSE1 = "adresse1";
        private static string ADRESSE2 = "adresse2";
        private static string ALT_ID = "alt_id";
        private static string BIRTH = "birth";
        private static string EMAIL = "email";
        private static string ID = "id";
        private static string KOEN = "koen";
        private static string MOBIL = "mobil";
        private static string NAVN = "navn";
        private static string OFF_TLF = "off_tlf";
        private static string OFF_EMAIL = "off_email";
        private static string OFF_MOBIL = "off_mobil";
        private static string OFF_NAVN = "off_navn";
        private static string POSTNR = "postnr";
        private static string POSTNRBY = "postnr_by";
        private static string SLETTET = "slettet";
        private static string TLF = "tlf";
        



        // TODO 3: Change interface to Stream to enable for unit testing

        public List<ConventusMedlem> getMembersFromConventus()
        {
            // Create a request for the URL.
            // TODO: Read from configuration file with forening and key as seperate parameters.
            string url = "https://www.conventus.dk/dataudv/api/adressebog/get_medlemmer.php";
            string key = "xxx"; // See value in your conventus administration page.
            string forening = "xxx"; // See value in your conventus administration page.
            
            WebRequest request = WebRequest.Create(url + "?forening=" + forening + "&key=" + key + "&type=person,medlem");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Load returned xml into xml-document for parsing
            XmlDocument xmldoc = new XmlDocument();
            Stream inputStream = response.GetResponseStream();

            return getMembersFromXml(inputStream);

        }


        /// <summary>
        /// Returns information regarding all members in Conventus.
        /// Extra properties supported from Conventus but not implemented here include
        /// </summary>
        /// <returns>A list of conventus members. Members that failed parsing are not returned.</returns>
        /// <exception cref="System.Xml.XmlException">XmlException if the argument cannot be parsed</exception>
        public List<ConventusMedlem> getMembersFromXml(Stream conventusXmlStream)
        {

            logger.Trace("getMembersFromXml called");
            

           // Load xml into xml-document for parsing
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.Load(conventusXmlStream);
            }
            catch (XmlException ex)
            {
                logger.LogException(LogLevel.Error, "Exception: Unable to parse conventus xml.", ex);
                throw ex;
            }


            logger.Debug(xmldoc);

            List<ConventusMedlem> result = new List<ConventusMedlem>();

            // Loop through members and create member objects
            XmlNodeList memberNodes = xmldoc.SelectNodes("/conventus/medlemmer/medlem");
            foreach (XmlNode member in memberNodes)
            {
               result.Add(createMember(member));
            }
            return result;
        }



        /// <summary>
        /// Create a member object for a single member node in the xml-structure
        /// </summary>
        /// <param name="member">An Xml node representing a single member</param>
        /// <returns>An object with the parameters set. Empty or undefinded values are set to null for integers and to empty string for strings, booleans are set to <code>false</code>.</returns>
        private ConventusMedlem createMember(XmlNode member)
        {

            // TODO 2: Robustness: Catch and log exceptions on parsing of single members

            // Create new ConventusMedlem object
            ConventusMedlem cm = new ConventusMedlem();

            // Conventus is expected to return an int value (non null) for all members
            cm.Id = int.Parse(member[ID].InnerText);


            // Adresse1 is not mandatory but logged as a warning if not present
            if (member[ADRESSE1] != null)
            {
                cm.Adresse1 = setValueOrEmpty(member[ADRESSE1].InnerText);
            }
            else
            {
                logger.Warn("MemberID " + cm.Id + " has no Adresse1 defined");
            }


            if (member[ADRESSE2] != null) {
                cm.Adresse2 = setValueOrEmpty(member[ADRESSE2].InnerText);
            }

            if (member[ALT_ID] != null)
            {
                cm.AltID = setValueOrNull(member[ALT_ID].InnerText);
            }

            // Email is not mandatory but logged as a warning if not present
            if (member[EMAIL] != null)
            {
                cm.Email = setValueOrEmptyWith0AsEmpty(member[EMAIL].InnerText);
            }
            else
            {
                logger.Warn("MemberID " + cm.Id + " has no email defined");
            }

            if (member[BIRTH] != null)
            {
                cm.Foedselsdato = setDateValueOrNull(member[BIRTH].InnerText);
            }
            else
            {
                cm.Foedselsdato = null;
                logger.Warn("MemberID " + cm.Id + " has no birth defined");
            }

            
            // Set the sex of the member
            if (member[KOEN] != null)
            {
                string sexString = member[KOEN].InnerText;
                switch (sexString)
                {
                    case "kvinde":
                        cm.Koen = ConventusMedlem.KoenTypes.Kvinde;
                        break;
                    case "mand":
                        cm.Koen = ConventusMedlem.KoenTypes.Mand;
                        break;
                    default:
                        cm.Koen = ConventusMedlem.KoenTypes.Ukendt;
                        logger.Warn("MemberID " + cm.Id + " has no sex defined. It has been set to Unknown");
                        break;
                }
            }
            else
            {
                cm.Koen = ConventusMedlem.KoenTypes.Ukendt;
                logger.Warn("MemberID " + cm.Id + " has no sex defined. It has been set to Unknown");
            }

            // Mobil is not mandatory but a warning is logged later on if no phone number is present for the member.
            if (member[MOBIL] != null)
            {
                cm.Mobilnummer = setValueOrEmptyWith0AsEmpty(member[MOBIL].InnerText);
            }
            

            if (member[NAVN] != null)
            {
                cm.Navn = setValueOrEmpty(member[NAVN].InnerText);
            }
            else
            {
                logger.Warn("MemberID " + cm.Id + " has no name defined. It has been set to empty");
            }

            cm.OffentligAdresse = isPublicAdresss(member);

            if (member[OFF_EMAIL] != null)
            {
                cm.OffentligEmail = setValueOrFalse(member[OFF_EMAIL].InnerText);
            }
            else
            {
                cm.OffentligEmail = false;
            }

            if (member[OFF_MOBIL] != null) {
                cm.OffentligMobil = setValueOrFalse(member[OFF_MOBIL].InnerText);
            } else {
                cm.OffentligMobil = false;
            }


            if (member[OFF_NAVN] != null)
            {
                cm.OfffentligNavn = setValueOrFalse(member[OFF_NAVN].InnerText);
            }
            else
            {
                cm.OffentligMobil = false;
            }

            if (member[OFF_TLF] != null) {
                cm.OffentligTelefon = setValueOrFalse(member[OFF_TLF].InnerText);
            } else {
                cm.OffentligTelefon = false;
            }

            if (member[POSTNR] != null) {
                cm.Postnummer = setEmptyIfzero(member[POSTNR].InnerText);
            } else {
                cm.Postnummer = string.Empty;
            }

            if (member[POSTNRBY] != null) {
                cm.PostnummerBy = setValueOrEmpty(member[POSTNRBY].InnerText);
            } else {
                cm.PostnummerBy = string.Empty;
            }

            if (member[SLETTET] != null) {
                cm.Slettet = setValueOrFalse(member[SLETTET].InnerText);
            } else {
                cm.Slettet = false;
                logger.Error("MemberID " + cm.Id + " has no Slettet defined. It has been set to not slettet");

            }
            
            if (member[TLF] != null) {
                cm.Telefonnummer = setValueOrEmptyWith0AsEmpty(member[TLF].InnerText);
            }

            // We should have at least one phone number for the member. If not log a warning
            if (string.IsNullOrEmpty(cm.Telefonnummer) && string.IsNullOrEmpty(cm.Mobilnummer)) {
                logger.Warn("MemberID " + cm.Id + " has neither phone or mobile defined.");
            }

            return cm;
        }



        /// 
        /// <summary>
        ///  Returns the string as either an int value or a null value. null is returned in case of any exception in the conversion.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>int value or null</returns>
        private int? setValueOrNull(string s)
        {

            try
            {
                return int.Parse(s);
            }
            catch (Exception)
            {
                // Set to null in case of exception
                return null;
            }
        }

        
        /// <summary>
        /// Helper function to set data values to null if they cannot be parsed.
        /// </summary>
        /// <param name="s">Date string</param>
        /// <returns>null or the string parsed as a date</returns>
        private DateTime? setDateValueOrNull(string s)
        {
            // If the string is null or empty return null
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            // If the date can be parsed - return a date - otherwise return null
            DateTime result;
            if (DateTime.TryParse(s, out result))
            {
                return result;
            } else {
                return null;
            }

        }

        private string setValueOrEmptyWith0AsEmpty(string s)
        {
            if (s == null)
            {
                return string.Empty;
            }

            // Sometimes conventus sets an undefined value to 0
            switch (s) {
                case "0" : 
                    return string.Empty;
                default :
                    return s;
            }
        }


        private string setValueOrEmpty(string s)
        {
            if (s == null)
            {
                return string.Empty;
            }
            else
            {
                return s;
            }
        }

        private string setEmptyIfzero(string s)
        {
            if (s == "0")
                return string.Empty;
            else
                return s;
        }


        /// <summary>
        /// An adress is considered public only if all fields are marked as public.
        /// If a field is not present, null or cannot be parsed as a boolean the adress is considered private.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Boolean isPublicAdresss(XmlNode node)
        {
            try
            {
                Boolean off_adresse1 = Boolean.Parse(node["off_adresse1"].InnerText);
                Boolean off_adresse2 = Boolean.Parse(node["off_adresse2"].InnerText);
                Boolean off_postnr = Boolean.Parse(node["off_postnr"].InnerText);

                return (off_adresse1 && off_adresse2 && off_postnr);
            }
            catch (Exception)
            {
                // TODO 2: Log exception if boolean cannot be parsed - signature should be extended with member node
                return false;
            }
        }

        private Boolean setValueOrFalse(string s)
        {
            try
            {
                return Boolean.Parse(s);
            }
            catch (Exception)
            {
                // TODO 2: Log any errors - signature should be extended with member-node.
                return false;
            }

        }
    }
    }
