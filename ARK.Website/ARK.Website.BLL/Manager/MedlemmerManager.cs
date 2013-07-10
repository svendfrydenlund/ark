using ARK.Website.BLL.BO;
using ARK.Website.Common.DTO;
using ARK.Website.Common.Enum;
using ARK.Website.Common.Interface;
using ARK.Website.Common.Manager;
using ARK.Website.EntityFramework.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.BLL.Manager
{
    public class MedlemmerManager : IJegHarInloggetMedlemIDogArkID
    {
        #region Fields
        private const string INLOGGET_MEDLEM_NOEGLE = "INDLOGGET_MEDLEM_NOEGLE";

        private const string LOG_IND_NOGLEORD = "LOG IND";
        private const string LOG_IND_ARKID = "Medlem logget ind [ArkID={0};Status={1}]";
        private const string LOG_IND_ARKID_FEJLET = "Medlem fejlet logget ind [ArkID={0};Status={1}]";

        private const string SYNKRONISERING_REGNSKAB_OG_MEDLEMMER_NOEGLEORD = "Synkronisering regnskab og medlemmer";
        #endregion

        #region Properties
        public static IndloggetMedlemBO IndloggetMedlem
        {
            get
            {
                return CacheManager.GetValue<IndloggetMedlemBO>(INLOGGET_MEDLEM_NOEGLE);
            }
            private set
            {
                CacheManager.SetValue(INLOGGET_MEDLEM_NOEGLE, value);
            }
        }

        public int? IndloggetMedlemID
        {
            get
            {
                int? inloggetMedlemID = null;
                IndloggetMedlemBO inloggetMedlem = MedlemmerManager.IndloggetMedlem;
                if (inloggetMedlem != null)
                {
                    inloggetMedlemID = inloggetMedlem.Data.ID;
                }
                return inloggetMedlemID;
            }
        }

        public int? IndloggetMedlemArkID
        {
            get
            {
                int? inloggetMedlemArkID = null;
                IndloggetMedlemBO inloggetMedlem = MedlemmerManager.IndloggetMedlem;
                if (inloggetMedlem != null)
                {
                    inloggetMedlemArkID = inloggetMedlem.Data.ArkID;
                }
                return inloggetMedlemArkID;
            }
        }
        #endregion

        #region Private methods
        internal static bool HarRegnskabsmedlemOgKendtRegnskabsmedlemSammeData(RegnskabsmedlemDTO regnskabsmedlem, Regnskabsmedlem kendtRegnskabsmedlem)
        {
            return
                regnskabsmedlem.Adresse == kendtRegnskabsmedlem.Adresse &&
                regnskabsmedlem.AdresseBy == kendtRegnskabsmedlem.AdresseBy &&
                regnskabsmedlem.AdressePostNummer == kendtRegnskabsmedlem.AdressePostNummer &&
                regnskabsmedlem.ArkID == kendtRegnskabsmedlem.ArkID &&
                regnskabsmedlem.EMailAdresse == kendtRegnskabsmedlem.EMailAdresse &&
                regnskabsmedlem.Foedselsdato == kendtRegnskabsmedlem.Foedselsdato &&
                regnskabsmedlem.Koen == kendtRegnskabsmedlem.Koen &&
                regnskabsmedlem.MobilNummer == kendtRegnskabsmedlem.MobilNummer &&
                regnskabsmedlem.Navn == kendtRegnskabsmedlem.Navn &&
                regnskabsmedlem.Status == kendtRegnskabsmedlem.Status;
        }

        internal static void OverskrivMedRegnskabsmedlemsdata(RegnskabsmedlemDTO regnskabsmedlem, Regnskabsmedlem kendtRegnskabsmedlem)
        {
            kendtRegnskabsmedlem.Adresse = regnskabsmedlem.Adresse;
            kendtRegnskabsmedlem.AdresseBy = regnskabsmedlem.AdresseBy;
            kendtRegnskabsmedlem.AdressePostNummer = regnskabsmedlem.AdressePostNummer;
            kendtRegnskabsmedlem.ArkID = regnskabsmedlem.ArkID;
            kendtRegnskabsmedlem.EMailAdresse = regnskabsmedlem.EMailAdresse;
            kendtRegnskabsmedlem.Foedselsdato = regnskabsmedlem.Foedselsdato;
            kendtRegnskabsmedlem.Koen = regnskabsmedlem.Koen;
            kendtRegnskabsmedlem.MobilNummer = regnskabsmedlem.MobilNummer;
            kendtRegnskabsmedlem.Navn = regnskabsmedlem.Navn;
            kendtRegnskabsmedlem.Status = regnskabsmedlem.Status;
        }

        internal static void OverskrivMedRegnskabsmedlemsdata(RegnskabsmedlemDTO regnskabsmedlem, Medlem medlem)
        {
            medlem.Adresse = regnskabsmedlem.Adresse;
            medlem.AdresseBy = regnskabsmedlem.AdresseBy;
            medlem.AdressePostNummer = regnskabsmedlem.AdressePostNummer;
            medlem.ArkID = regnskabsmedlem.ArkID;
            medlem.EMailAdresse = regnskabsmedlem.EMailAdresse;
            medlem.Foedselsdato = regnskabsmedlem.Foedselsdato;
            medlem.Koen = regnskabsmedlem.Koen;
            medlem.MobilNummer = regnskabsmedlem.MobilNummer;
            medlem.Navn = regnskabsmedlem.Navn;
            medlem.Status = regnskabsmedlem.Status;
        }

        private bool ErRegnskabsmedlemmerLaesningValid(List<RegnskabsmedlemDTO> regnskabsmedlemmer)
        {
            int antalAktiveRegnskabsmedlemmer = regnskabsmedlemmer.Count(medlemsItem => medlemsItem.Status == Common.Enum.MedlemsstatusEnum.Aktiv);
            return antalAktiveRegnskabsmedlemmer > 100;
        }
        #endregion

        #region Methoder
        public void SynkroniserRegnskabsmedlemmerOgMedlemmer(bool validerLaesteRegnskabsmedlemmer = true)
        {
            StringBuilder logBeskedBygger = new StringBuilder();

            //¤¤¤ Tegnforklaring
            //¤¤¤ Tekst som står efter '//¤¤¤' er en kommentar
            //¤¤¤ Tekst som står efter '//' skal konverteres til kode

            //¤¤¤ Tre entiteter er vigtige i denne sammenhæng:
            //¤¤¤ Regnskabsmedlem: Læst direkte i regnskabssystemet
            //¤¤¤ KendtRegnskabsmedlem: Persisteret data fra siste gang regnskabssystemet leverede et opdateret Regnskabsmedlem
            //¤¤¤ Medlem: Brugeren (aktiveret eller ej) af websitet.
            //¤¤¤
            //¤¤¤ Disse tre entiteter har datafelter til fælles som defineret i ARK.Website.Common.RegnskabsmedlemDTO
            //¤¤¤ At to entiteter er ens betyder at datafelterne alle har samme værdi

            //¤¤¤ Status er vigtig i denne sammenhæng (ARK.Website.Common.Enum.MedlemsstatusEnum)
            //¤¤¤ Værdi IkkeAktiveret: Bruger som kan tilgå logbog og kan ses på websitet, men som aldrig har været logget på websitet
            //¤¤¤ Værdi Aktiv: Bruger som kan tilgå logbog og website
            //¤¤¤ Værdi Inaktiv: Bruger som kan tilgå website men ikke skrive til logbog (kan ikke få lov at ro/padle)
            //¤¤¤ Værdi Gammel: Bruger som kan tilgå hverken website eller logbog, men som stadig figurerer i rostatistik og website historik

            List<RegnskabsmedlemDTO> regnskabsmedlemmer = new List<RegnskabsmedlemDTO>();
            try
            {
                regnskabsmedlemmer = KomponentManager.RegnskabsmedlemsManager.HentRegnskabsmedlemmer();
            }
            catch (Exception exception)
            {
                logBeskedBygger.AppendLine("Læsning af regnskabsmedlemmer fejlede: " + exception.ToString() + Environment.NewLine);
            }

            //¤¤¤ Valider regnskabsmedlemmer så forfejlede retursvar ikke fortsætter. Kunne eksempelvis være antallet af læste aktive regnskabsmedlemmer
            int antalAktiveRegnskabsmedlemmer = regnskabsmedlemmer.Count(medlemsItem => medlemsItem.Status == Common.Enum.MedlemsstatusEnum.Aktiv);
            bool erRegnskabsmedlemmerLaesningValid = true;
            if (validerLaesteRegnskabsmedlemmer)
            {
                ErRegnskabsmedlemmerLaesningValid(regnskabsmedlemmer);
            }
            logBeskedBygger.AppendLine("Regnskabsmedlemmer læst: [Valid læsning:" + erRegnskabsmedlemmerLaesningValid + ";Antal:" + regnskabsmedlemmer.Count + ";Medlemsstatus-Aktiv:" + antalAktiveRegnskabsmedlemmer + "]" + Environment.NewLine);

            StringBuilder opdateretMedlemsbesked = new StringBuilder();
            if (erRegnskabsmedlemmerLaesningValid)
            {
                using (ARK.Website.EntityFramework.Main.ArkDatabase db = new EntityFramework.Main.ArkDatabase())
                {
                    List<Medlem> medlemmer = db.Medlems.ToList();
                    List<Regnskabsmedlem> kendteRegnskabsmedlemmer = db.Regnskabsmedlems.ToList();

                    foreach (RegnskabsmedlemDTO regnskabsmedlem in regnskabsmedlemmer)
                    {
                        bool erOpdateret = false;
                        opdateretMedlemsbesked.Clear();
                        int arkID = regnskabsmedlem.ArkID;
                        opdateretMedlemsbesked.AppendLine("Regnskabsmedlem ArkID = " + arkID);
                        try
                        {
                            Regnskabsmedlem kendtRegnskabsmedlem = kendteRegnskabsmedlemmer.FirstOrDefault(regnskabsmedlemItem => regnskabsmedlemItem.ArkID == arkID);
                            Medlem medlem = medlemmer.FirstOrDefault(medlemItem => medlemItem.ArkID == arkID);

                            if (medlem == null || kendtRegnskabsmedlem == null)
                            {
                                if (medlem != null || kendtRegnskabsmedlem != null)
                                {
                                    throw new Exception("Fejl i modellen [ArkID=" + arkID + "]: Medlem = " + (medlem == null ? "Null" : "Not Null") + "; KendtRegnskabsmedlem = " + (kendtRegnskabsmedlem == null ? "Null" : "Not Null"));
                                }
                            }

                            if (medlem != null)
                            {
                                //¤¤¤ Endnu ikke aktiverede medlemmer holdes synkrone med regnskabssystemet
                                //¤¤¤ Gamle medlemmer, som kommmer tilbage i systemet, bliver behandlet som nye

                                MedlemsstatusEnum oprindeligMedlemsstatus = medlem.Status;
                                MedlemsstatusEnum oprindeligKendtRegnskabsmedlemsstatus = kendtRegnskabsmedlem.Status;
                                bool medlemsdataOverskrevet = false;
                                bool kendtRegnskabsmedlemsdataOverskrevet = false;
                                bool regnskabsmedlemDataEns = HarRegnskabsmedlemOgKendtRegnskabsmedlemSammeData(regnskabsmedlem, kendtRegnskabsmedlem);
                                if (!regnskabsmedlemDataEns)
                                {
                                    #region Opdatering af eksisterende Medlem og KendtRegnskabsmedlem data
                                    switch (regnskabsmedlem.Status)
                                    {
                                        case MedlemsstatusEnum.Aktiv:
                                            {
                                                switch (oprindeligMedlemsstatus)
                                                {
                                                    case MedlemsstatusEnum.Aktiv:
                                                        {
                                                            //Håndteret af bruger logon
                                                        } break;
                                                    case MedlemsstatusEnum.Inaktiv:
                                                        {
                                                            //Status i Regnskabsmedlem - tabellen skal opdateres
                                                            //Resten af ændringer i regnskabsmedlemsdata skal håndteres af bruger logon
                                                            kendtRegnskabsmedlem.Status = regnskabsmedlem.Status;
                                                            medlem.Status = regnskabsmedlem.Status;
                                                        } break;
                                                    case MedlemsstatusEnum.IkkeAktiveret:
                                                    case MedlemsstatusEnum.Gammel:
                                                        {
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, kendtRegnskabsmedlem);
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, medlem);
                                                            medlem.Status = MedlemsstatusEnum.IkkeAktiveret;

                                                            medlemsdataOverskrevet = true;
                                                            kendtRegnskabsmedlemsdataOverskrevet = true;
                                                        } break;
                                                    default:
                                                        {
                                                            throw new NotSupportedException("Medlem.Status = " + oprindeligMedlemsstatus);
                                                        }
                                                }
                                            } break;
                                        case MedlemsstatusEnum.Inaktiv:
                                            {
                                                switch (oprindeligMedlemsstatus)
                                                {
                                                    case MedlemsstatusEnum.Inaktiv:
                                                        {
                                                            //Håndteret af bruger logon
                                                        } break;
                                                    case MedlemsstatusEnum.Aktiv:
                                                        {
                                                            //Status i Regnskabsmedlem - tabellen skal opdateres
                                                            //Resten af ændringer i regnskabsmedlemsdata skal håndteres af bruger logon
                                                            kendtRegnskabsmedlem.Status = regnskabsmedlem.Status;
                                                            medlem.Status = regnskabsmedlem.Status;
                                                        } break;
                                                    case MedlemsstatusEnum.IkkeAktiveret:
                                                    case MedlemsstatusEnum.Gammel:
                                                        {
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, kendtRegnskabsmedlem);
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, medlem);
                                                            medlem.Status = MedlemsstatusEnum.IkkeAktiveret;

                                                            medlemsdataOverskrevet = true;
                                                            kendtRegnskabsmedlemsdataOverskrevet = true;
                                                        } break;
                                                    default:
                                                        {
                                                            throw new NotSupportedException("Medlem.Status = " + oprindeligMedlemsstatus);
                                                        }
                                                }
                                            } break;
                                        case MedlemsstatusEnum.Gammel:
                                            {
                                                switch (oprindeligMedlemsstatus)
                                                {
                                                    case MedlemsstatusEnum.IkkeAktiveret:
                                                        {
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, kendtRegnskabsmedlem);
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, medlem);

                                                            medlemsdataOverskrevet = true;
                                                            kendtRegnskabsmedlemsdataOverskrevet = true;
                                                        } break;
                                                    case MedlemsstatusEnum.Aktiv:
                                                    case MedlemsstatusEnum.Inaktiv:
                                                    case MedlemsstatusEnum.Gammel:
                                                        {
                                                            OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, kendtRegnskabsmedlem);
                                                            medlem.Status = regnskabsmedlem.Status;

                                                            kendtRegnskabsmedlemsdataOverskrevet = true;
                                                        } break;
                                                    default:
                                                        {
                                                            throw new NotSupportedException("Medlem.Status = " + oprindeligMedlemsstatus);
                                                        }
                                                }
                                            } break;
                                        default:
                                            {
                                                throw new NotSupportedException("Regnskabsmedlem.Status = " + regnskabsmedlem.Status);
                                            }
                                    }
                                    #endregion
                                }

                                #region Logbesked generering
                                if (medlemsdataOverskrevet)
                                {
                                    opdateretMedlemsbesked.AppendLine("EKSISTERENDE MEDLEM OVERSKREVET FRA REGNSKABSSYSTEMET");
                                    erOpdateret = true;
                                }

                                if (medlem.Status != oprindeligMedlemsstatus)
                                {
                                    opdateretMedlemsbesked.AppendLine("MEDLEMSSTATUS ÆNDRET " + oprindeligMedlemsstatus + "=>" + medlem.Status);
                                    erOpdateret = true;
                                }

                                if (kendtRegnskabsmedlemsdataOverskrevet)
                                {
                                    opdateretMedlemsbesked.AppendLine("KENDT REGNSKABSMEDLEM OVERSKREVET FRA REGNSKABSSYSTEMET");
                                    erOpdateret = true;
                                }


                                if (kendtRegnskabsmedlem.Status != oprindeligKendtRegnskabsmedlemsstatus &&
                                    !kendtRegnskabsmedlemsdataOverskrevet)
                                {
                                    opdateretMedlemsbesked.AppendLine("KENDT REGNSKABSMEDLEM STATUS ÆNDRET " + oprindeligKendtRegnskabsmedlemsstatus + "=>" + kendtRegnskabsmedlem.Status);
                                    erOpdateret = true;
                                }
                                #endregion
                            }
                            else
                            {
                                #region Oprettelse af nyt Medlem og tilhørende KendtRegnskabsmedlem
                                medlem = new Medlem();
                                medlem.Rostatistik = new Rostatistik();
                                db.Medlems.Add(medlem);

                                kendtRegnskabsmedlem = new Regnskabsmedlem();
                                db.Regnskabsmedlems.Add(kendtRegnskabsmedlem);

                                OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, medlem);
                                OverskrivMedRegnskabsmedlemsdata(regnskabsmedlem, kendtRegnskabsmedlem);

                                if (regnskabsmedlem.Status != MedlemsstatusEnum.Gammel)
                                {
                                    medlem.Status = MedlemsstatusEnum.IkkeAktiveret;
                                }
                                else
                                {
                                    medlem.Status = MedlemsstatusEnum.Gammel;
                                }
                                #endregion

                                erOpdateret = true;
                                opdateretMedlemsbesked.AppendLine("NYT MEDLEM - STATUS = " + medlem.Status);
                            }

                            if (erOpdateret)
                            {
                                logBeskedBygger.AppendLine(opdateretMedlemsbesked.ToString());
                            }
                            db.SaveChanges();
                        }
                        catch (Exception exception)
                        {
                            logBeskedBygger.AppendLine("Fejl under kørsel af medlem ArkID = " + arkID + Environment.NewLine + exception.ToString() + Environment.NewLine);
                        }
                    }

                    try
                    {
                        //¤¤¤ de medlemmer som ikke længere får returneret et tilhørende regnskabsmedlem overgår til status gammel.
                        foreach (Medlem medlem in medlemmer.Where(medlemItem => medlemItem.Status != MedlemsstatusEnum.Gammel))
                        {
                            int arkID = medlem.ArkID;
                            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemmer.FirstOrDefault(regnskabsmedlemItem => regnskabsmedlemItem.ArkID == arkID);
                            if (regnskabsmedlem == null)
                            {
                                MedlemsstatusEnum oprindeligMedlemsstatus = medlem.Status;
                                medlem.Status = MedlemsstatusEnum.Gammel;
                                opdateretMedlemsbesked.AppendLine("Regnskabsmedlem ArkID = " + arkID);
                                opdateretMedlemsbesked.AppendLine("Regnskabsmedlem ikke eksisterende");
                                opdateretMedlemsbesked.AppendLine("MEDLEMSSTATUS ÆNDRET " + oprindeligMedlemsstatus + "=>" + medlem.Status);
                            }
                        }
                        db.SaveChanges();
                    }
                    catch (Exception exception)
                    {
                        logBeskedBygger.AppendLine("Fejl ved ændring af medlemmer status Gammel ved manglende regnskabsmedlem" + Environment.NewLine + exception.ToString() + Environment.NewLine);
                    }
                }
            }

            KomponentManager.LoggingManager.LogBesked(SYNKRONISERING_REGNSKAB_OG_MEDLEMMER_NOEGLEORD, logBeskedBygger.ToString());
        }

        public MedlemLogindStatusEnum TryLogMedlemIndMedArkID(int arkID)
        {
            IndloggetMedlem = null;
            IndloggetMedlemBO indloggetMedlem = new IndloggetMedlemBO(arkID);
            if (indloggetMedlem.LogindStatus == MedlemLogindStatusEnum.Aktivering ||
                indloggetMedlem.LogindStatus == MedlemLogindStatusEnum.Succes)
            {
                IndloggetMedlem = indloggetMedlem;
                KomponentManager.LoggingManager.LogBesked(LOG_IND_NOGLEORD, String.Format(LOG_IND_ARKID,arkID.ToString(),indloggetMedlem.LogindStatus.ToString()));
            }
            else
            {
                KomponentManager.LoggingManager.LogBesked(LOG_IND_NOGLEORD, String.Format(LOG_IND_ARKID_FEJLET, arkID.ToString(), indloggetMedlem.LogindStatus.ToString()));
            }
            
            return indloggetMedlem.LogindStatus;
        }
        #endregion
    }
}
