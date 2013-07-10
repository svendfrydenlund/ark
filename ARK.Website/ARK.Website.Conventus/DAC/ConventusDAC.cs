using ARK.Website.Common.DTO;
using ARK.Website.Common.Enum;
using ARK.Website.Common.Manager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ARK.Website.Conventus.DAC
{
    internal class ConventusDAC
    {
        #region Private felter
        private const string FORBINDELSESPUNKT = @"https://www.conventus.dk";
        private const string MEDLEMMERSFORESPOERGSEL = @"/dataudv/api/adressebog/get_medlemmer.php";
        private const string MEDLEMSFORESPOERGSEL = @"/dataudv/api/adressebog/get_medlem.php";
        private const string MEDLEMSFORESPOERGSELSPARAMETRE = @"type=person,medlem&slettet=true,false";
        private const string ENKELTMEDLEMFORESPOERGSELSPARAMETRE = MEDLEMSFORESPOERGSELSPARAMETRE + @"&id={0}";
        private const string FORENINGSIDENTIFIKATOR = @"forening={0}&key={1}";

        private const string SETTINGS_FORENING = "ConventusForening";
        private const string SETTINGS_FORENINGSNOEGLE = "ConventusForeningsNoegle";

        private const string XML_DOKUMENT_MEDLEMMERSNODENAVN = @"/conventus/medlemmer/medlem";
        private const string XML_DOKUMENT_MEDLEMSNODENAVN = @"/conventus/medlem";

        private const string MEDLEMSFELT_ADRESSE1 = "adresse1";
        private const string MEDLEMSFELT_ADRESSE2 = "adresse2";
        private const string MEDLEMSFELT_ALT_ID = "alt_id";
        private const string MEDLEMSFELT_BIRTH = "birth";
        private const string MEDLEMSFELT_EMAIL = "email";
        private const string MEDLEMSFELT_ID = "id";
        private const string MEDLEMSFELT_KOEN = "koen";
        private const string MEDLEMSFELT_MOBIL = "mobil";
        private const string MEDLEMSFELT_NAVN = "navn";
        private const string MEDLEMSFELT_OFF_TLF = "off_tlf";
        private const string MEDLEMSFELT_OFF_EMAIL = "off_email";
        private const string MEDLEMSFELT_OFF_MOBIL = "off_mobil";
        private const string MEDLEMSFELT_OFF_NAVN = "off_navn";
        private const string MEDLEMSFELT_POSTNR = "postnr";
        private const string MEDLEMSFELT_POSTNRBY = "postnr_by";
        private const string MEDLEMSFELT_SLETTET = "slettet";
        private const string MEDLEMSFELT_TLF = "tlf";

        private string _foreningsID = null;
        private string _foreningsNoegle = null;
        #endregion

        #region Konstruktion
        internal ConventusDAC()
        {
            bool settingHarForening = ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem == SETTINGS_FORENING);
            bool settingHarForeningsnoegle = ConfigurationManager.AppSettings.AllKeys.Any(keyItem => keyItem == SETTINGS_FORENINGSNOEGLE);
            if ((!settingHarForening) || (!settingHarForeningsnoegle))
            {
                throw new Exception("App.Config eller Web.Config skal have noegler " + SETTINGS_FORENING + " og " + SETTINGS_FORENINGSNOEGLE + " med vaerdier");
            }
            _foreningsID = ConfigurationManager.AppSettings[SETTINGS_FORENING];
            _foreningsNoegle = ConfigurationManager.AppSettings[SETTINGS_FORENINGSNOEGLE];
        }
        #endregion

        #region Private metoder
        private string HentMedlemmersforespoergsel()
        {
            string medlemsForespoergsel =
                FORBINDELSESPUNKT +
                MEDLEMMERSFORESPOERGSEL + "?" +
                String.Format(FORENINGSIDENTIFIKATOR, _foreningsID, _foreningsNoegle) + "&" +
                MEDLEMSFORESPOERGSELSPARAMETRE;
            return medlemsForespoergsel;
        }

        private string HentMedlemsforespoergsel(int arkID)
        {
            string medlemsForespoergsel =
                FORBINDELSESPUNKT +
                MEDLEMSFORESPOERGSEL + "?" +
                String.Format(FORENINGSIDENTIFIKATOR, _foreningsID, _foreningsNoegle) + "&" +
                String.Format(ENKELTMEDLEMFORESPOERGSELSPARAMETRE, arkID.ToString());
            return medlemsForespoergsel;
        }

        private List<RegnskabsmedlemDTO> KonverterRegnskabsmedlemmerFraXmlDocument(XmlDocument document)
        {
            List<RegnskabsmedlemDTO> regnskabsmedlemmer = new List<RegnskabsmedlemDTO>();
            XmlNodeList medlemsnoder = document.SelectNodes(XML_DOKUMENT_MEDLEMMERSNODENAVN);
            foreach (XmlNode medlemsnode in medlemsnoder)
            {
                try
                {
                    RegnskabsmedlemDTO regnskabsMedlem = KonverterRegnskabsmedlemFraXmlNode(medlemsnode);
                    regnskabsmedlemmer.Add(regnskabsMedlem);
                }
                catch (Exception exception)
                {
                    KomponentManager.LoggingManager.LogException(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, exception);
                }
            }
            return regnskabsmedlemmer;
        }

        private RegnskabsmedlemDTO KonverterRegnskabsmedlemFraXmlDocument(XmlDocument document)
        {
            RegnskabsmedlemDTO regnskabsmedlem = null;
            XmlNodeList medlemsnoder = document.SelectNodes(XML_DOKUMENT_MEDLEMSNODENAVN);
            if (medlemsnoder.Count > 1)
            {
                throw new Exception("Conventus returnerede multiple entiteter på single <id/>");
            }
            foreach (XmlNode medlemsnode in medlemsnoder)
            {
                try
                {
                    regnskabsmedlem = KonverterRegnskabsmedlemFraXmlNode(medlemsnode);
                }
                catch (Exception exception)
                {
                    KomponentManager.LoggingManager.LogException(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, exception);
                }
            }
            return regnskabsmedlem;
        }

        private RegnskabsmedlemDTO KonverterRegnskabsmedlemFraXmlNode(XmlNode medlemsnode)
        {
            RegnskabsmedlemDTO regnskabsmedlem = new RegnskabsmedlemDTO();

            #region ID - Conventus is expected to return an int value (non null) for all members - THROWS EXCEPTION
            int? ID = XmlNodeValueToNullableInt(medlemsnode, MEDLEMSFELT_ID);
            if (ID.HasValue)
            {
                regnskabsmedlem.ArkID = ID.Value;
            }
            else
            {
                throw new Exception("Medlemsnode: Intet <" + MEDLEMSFELT_ID + ">" + Environment.NewLine + medlemsnode.InnerText);
            }
            #endregion

            #region Status - TODO - THROWS EXCEPTION
            bool? slettet = XmlNodeValueToNullableBoolean(medlemsnode, MEDLEMSFELT_SLETTET);
            if (!slettet.HasValue)
            {
                throw new Exception(HentBesked(regnskabsmedlem.ArkID, "Felt ikke valid", MEDLEMSFELT_SLETTET, medlemsnode.InnerText));
            }
            else
            {
                //TODO: Refine
                if (slettet.Value)
                {
                    regnskabsmedlem.Status = MedlemsstatusEnum.Gammel;
                }
                else
                {
                    regnskabsmedlem.Status = MedlemsstatusEnum.Aktiv;
                }
            }
            #endregion

            #region Adresse - We will only use one address on the website. A Conventus member is only valid if such is provided. - THROWS EXCEPTION
            string adresse1 = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_ADRESSE1);
            string adresse2 = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_ADRESSE2);
            if (adresse1 != null)
            {
                regnskabsmedlem.Adresse = adresse1;
            }
            else if (adresse2 != null)
            {
                regnskabsmedlem.Adresse = adresse2;
            }
            else
            {
                throw new Exception(HentBesked(regnskabsmedlem.ArkID, "Ingen valid adresse", MEDLEMSFELT_ADRESSE1 + "> eller <" + MEDLEMSFELT_ADRESSE2, medlemsnode.InnerXml));
            }

            regnskabsmedlem.AdressePostNummer = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_POSTNR);
            if (regnskabsmedlem.AdressePostNummer == null)
            {
                throw new Exception(HentBesked(regnskabsmedlem.ArkID, "Intet valid postnummer", MEDLEMSFELT_POSTNR, medlemsnode.InnerXml));
            }

            regnskabsmedlem.AdresseBy = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_POSTNRBY);
            if (regnskabsmedlem.AdresseBy == null)
            {
                throw new Exception(HentBesked(regnskabsmedlem.ArkID, "Ingen valid by", MEDLEMSFELT_POSTNRBY, medlemsnode.InnerXml));
            }
            #endregion

            #region LOGGER ADVARSEL
            regnskabsmedlem.Navn = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_NAVN);
            if (regnskabsmedlem.Navn == null)
            {
                KomponentManager.LoggingManager.LogAdvarsel(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, HentBesked(regnskabsmedlem.ArkID, "Inget valid navn", MEDLEMSFELT_NAVN));
            }

            regnskabsmedlem.EMailAdresse = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_EMAIL);
            if (regnskabsmedlem.EMailAdresse == null)
            {
                KomponentManager.LoggingManager.LogAdvarsel(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, HentBesked(regnskabsmedlem.ArkID, "Ingen valid e-mail adresse", MEDLEMSFELT_EMAIL));
            }
            #endregion

            regnskabsmedlem.Foedselsdato = XmlNodeValueToNullableDateTime(medlemsnode, MEDLEMSFELT_BIRTH);
            regnskabsmedlem.Koen = XmlNodeValueToKoenEnum(medlemsnode, MEDLEMSFELT_KOEN);
            regnskabsmedlem.MobilNummer = XmlNodeValueToString(medlemsnode, MEDLEMSFELT_MOBIL);

            //TODO: Ignorerede felter
            //private const string MEDLEMSFELT_ALT_ID = "alt_id";
            //private const string MEDLEMSFELT_OFF_TLF = "off_tlf";
            //private const string MEDLEMSFELT_OFF_EMAIL = "off_email";
            //private const string MEDLEMSFELT_OFF_MOBIL = "off_mobil";
            //private const string MEDLEMSFELT_OFF_NAVN = "off_navn";
            //private const string MEDLEMSFELT_TLF = "tlf";

            return regnskabsmedlem;
        }

        private string XmlNodeValueToString(XmlNode medlemsnode, string noegle)
        {
            string vaerdi = null;
            XmlElement element = medlemsnode[noegle];
            if (element != null)
            {
                string innerText = element.InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    vaerdi = innerText;
                }
            }
            return vaerdi;
        }

        private int? XmlNodeValueToNullableInt(XmlNode medlemsnode, string noegle)
        {
            int? vaerdi = null;
            XmlElement element = medlemsnode[noegle];
            if (element != null)
            {
                string innerText = element.InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    vaerdi = Convert.ToInt32(innerText);
                }
            }
            return vaerdi;
        }

        private DateTime? XmlNodeValueToNullableDateTime(XmlNode medlemsnode, string noegle)
        {
            DateTime? vaerdi = null;
            XmlElement element = medlemsnode[noegle];
            if (element != null)
            {
                string innerText = element.InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    vaerdi = Convert.ToDateTime(innerText);
                }
            }
            return vaerdi;
        }

        private bool? XmlNodeValueToNullableBoolean(XmlNode medlemsnode, string noegle)
        {
            bool? vaerdi = null;
            XmlElement element = medlemsnode[noegle];
            if (element != null)
            {
                string innerText = element.InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    vaerdi = Convert.ToBoolean(innerText);
                }
            }
            return vaerdi;
        }

        private KoenEnum XmlNodeValueToKoenEnum(XmlNode medlemsnode, string noegle)
        {
            KoenEnum vaerdi = KoenEnum.Undefined;
            XmlElement element = medlemsnode[noegle];
            if (element != null)
            {
                string innerText = element.InnerText;
                if (!String.IsNullOrEmpty(innerText))
                {
                    switch (innerText)
                    {
                        case "kvinde":
                            vaerdi = KoenEnum.Kvinde;
                            break;
                        case "mand":
                            vaerdi = KoenEnum.Mand;
                            break;
                        default: break;
                    }
                }
            }
            return vaerdi;
        }

        private string HentBesked(int arkID, string besked, string medlemsfelt, string innerXml = null)
        {
            return "Medlemsnode [ArkID/" + MEDLEMSFELT_ID + "=" + arkID + "]: " + besked + " <" + medlemsfelt + ">" + (innerXml == null ? String.Empty : Environment.NewLine + innerXml);
        }
        #endregion

        #region Metoder
        internal List<RegnskabsmedlemDTO> HentRegnskabsmedlemmer()
        {
            List<RegnskabsmedlemDTO> regnskabsmedlemmer = new List<RegnskabsmedlemDTO>();
            try
            {
                WebRequest forespoergsel = WebRequest.Create(HentMedlemmersforespoergsel());
                HttpWebResponse svar = (HttpWebResponse)forespoergsel.GetResponse();
                Stream svarStream = svar.GetResponseStream();
                // Load returned xml into xml-document for parsing
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(svarStream);
                regnskabsmedlemmer = KonverterRegnskabsmedlemmerFraXmlDocument(xmlDocument);
            }
            catch (Exception exception)
            {
                KomponentManager.LoggingManager.LogException(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, exception);
                throw new Exception("Rethrown", exception);
            }
            return regnskabsmedlemmer;
        }

        internal RegnskabsmedlemDTO HentRegnskabsmedlem(int arkID)
        {
            RegnskabsmedlemDTO regnskabsmedlem = null;
            try
            {
                WebRequest forespoergsel = WebRequest.Create(HentMedlemsforespoergsel(arkID));
                HttpWebResponse svar = (HttpWebResponse)forespoergsel.GetResponse();
                Stream svarStream = svar.GetResponseStream();
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(svarStream);
                regnskabsmedlem = KonverterRegnskabsmedlemFraXmlDocument(xmlDocument);
            }
            catch (Exception exception)
            {
                KomponentManager.LoggingManager.LogException(MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, exception);
                throw new Exception("Rethrown", exception);
            }
            return regnskabsmedlem;
        }
        #endregion
    }
}
