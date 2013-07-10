using ARK.Website.Common.Enum;
using ARK.Website.Common.Manager;
using ARK.Website.EntityFramework.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.UnitTest.Common
{
    public static class UnitTestHelper
    {
        private static int _nytMedlemsNummer = 1;

        public static byte[] LaesBilleddata(string billedePath)
        {
            byte[] billedeData = null;

            return billedeData;
        }

        public static void SletAlt()
        {
            UnitTestHelper.SletTure();
            UnitTestHelper.SletBaade();
            UnitTestHelper.SletBegivenheder();
            UnitTestHelper.SletMedlemmer();
        }

        public static void SletMedlemmer()
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Medlem> medlemmer = db.Medlems.Include("Rostatistik").ToList();
                List<Regnskabsmedlem> kendteRegnskabsmedlemmer = db.Regnskabsmedlems.ToList();
                medlemmer.ForEach(medlemItem =>
                {
                    db.Rostatistiks.Remove(medlemItem.Rostatistik);
                    db.Medlems.Remove(medlemItem);
                });
                kendteRegnskabsmedlemmer.ForEach(regnskabsmedlemItem => db.Regnskabsmedlems.Remove(regnskabsmedlemItem));
                db.SaveChanges();
            }
        }

        public static void SletBegivenheder()
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Begivenhed> begivenheder = db.Begivenheds.ToList();
                begivenheder.ForEach(begivenhedsItem => db.Begivenheds.Remove(begivenhedsItem));
                db.SaveChanges();
            }
        }

        public static void SletTure()
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Turdeltager> list = db.Turdeltagers.ToList();
                list.ForEach(item => db.Turdeltagers.Remove(item));
                List<Tur> ture = db.Turs.ToList();
                ture.ForEach(turItem => db.Turs.Remove(turItem));
                db.SaveChanges();
            }
        }

        public static void SletBaade()
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Baad> list = db.Baads.ToList();
                list.ForEach(item => db.Baads.Remove(item));

                List<BaadType> list2 = db.BaadTypes.ToList();
                list2.ForEach(item => db.BaadTypes.Remove(item));

                List<BaadType> list3 = db.BaadTypes.ToList();
                list3.ForEach(item => db.BaadTypes.Remove(item));

                db.SaveChanges();
            }
        }

        public static Baad LavEnkeltBaad()
        {
            Baad baad = null;
            using (ArkDatabase db = new ArkDatabase())
            {
                baad = new Baad();
                baad.AntalPersoner = 1;
                baad.BaadType = new BaadType();
                baad.BaadType.BaadKategori = new BaadKategori();
                baad.BaadType.BaadKategori.Navn = "Kategori1";
                baad.BaadType.Navn = "Kategori1s baadtype1";
                baad.Navn = "Foerste baad";
                db.Baads.Add(baad);
                db.SaveChanges();
            }
            return baad;
        }

        public static void OpretTurPaaBaadOgMedlem(int medlemID, int baadID, DateTime tidspunkt, int turlaengde)
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                Tur tur = new Tur();
                tur.StartTidspunkt = tidspunkt;
                tur.AntalKilometer = turlaengde;
                tur.BaadID = baadID;
                Turdeltager turdeltager = new Turdeltager();
                turdeltager.MedlemID = medlemID;
                tur.Turdeltagers.Add(turdeltager);
                db.Turs.Add(tur);
                db.SaveChanges();
            }
        }

        public static void IndsaetXNytMedlem(int X, MedlemsstatusEnum? forceretStatus = null)
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                for (int i = 0; i < X; i++)
                {
                    Medlem medlem = new Medlem()
                    {
                        Adresse = "Adresse" + _nytMedlemsNummer,
                        AdresseBy = "By" + _nytMedlemsNummer,
                        AdressePostNummer = _nytMedlemsNummer.ToString(),
                        ArkID = _nytMedlemsNummer,
                        EMailAdresse = "EMail" + _nytMedlemsNummer,
                        Foedselsdato = new DateTime(1977 + _nytMedlemsNummer, 1, 1),
                        Koen = (_nytMedlemsNummer % 2 == 0 ? KoenEnum.Mand : KoenEnum.Kvinde),
                        MobilNummer = (22220000 + _nytMedlemsNummer).ToString(),
                        Navn = "Navn" + _nytMedlemsNummer,
                        Status = (forceretStatus.HasValue ? forceretStatus.Value : (MedlemsstatusEnum)((_nytMedlemsNummer % 3) + 2)),
                        Rostatistik = new Rostatistik()
                    };
                    _nytMedlemsNummer++;
                    db.Medlems.Add(medlem);
                }
                db.SaveChanges();
            }
        }

        public static int HentFoersteMedlemsArkID()
        {
            int arkID = 0;
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Medlem> medlemmer = db.Medlems.ToList();
                Medlem medlem = medlemmer.First();
                arkID = medlem.ArkID;
            }
            return arkID;
        }

        public static int HentFoersteMedlemsID()
        {
            int ID = 0;
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Medlem> medlemmer = db.Medlems.ToList();
                Medlem medlem = medlemmer.First();
                ID = medlem.ID;
            }
            return ID;
        }

        public static Medlem HentFoersteMedlem()
        {
            Medlem medlem = null;
            using (ArkDatabase db = new ArkDatabase())
            {
                List<Medlem> medlemmer = db.Medlems.Include("Rostatistik").ToList();
                medlem = medlemmer.First();
            }
            return medlem;
        }

        public static void OpdaterMedlemStatus(int arkID, MedlemsstatusEnum nyStatus)
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                Medlem medlem = db.Medlems.FirstOrDefault(medlemItem => medlemItem.ArkID == arkID);
                medlem.Status = nyStatus;
                db.SaveChanges();
            }
        }

        public static void OpdaterMedlemStatusOgNavn(int arkID, MedlemsstatusEnum nyStatus, string nytNavn)
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                Medlem medlem = db.Medlems.FirstOrDefault(medlemItem => medlemItem.ArkID == arkID);
                medlem.Status = nyStatus;
                medlem.Navn = nytNavn;
                db.SaveChanges();
            }
        }

        public static void InitierAlleKomponenterMedDefault()
        {
            CacheManager.KoereUnitTestOgBenytterStatiskCacheOgIkkeSession = true;
            KomponentManager.ApplikationKontekst = new ApplikationKontekstUnitTest();
            KomponentManager.JegHarIndloggetMedlemIDOgArkID = new JegHarIndloggetMedlemIDOgArkIDUnitTest();
            KomponentManager.LoggingManager = new LoggingManagerUnitTest();
            KomponentManager.RegnskabsmedlemsManager = new RegnskabsmedlemsManagerUnitTest();
            KomponentManager.EMailDistributoer = new EMailDistributoerUnitTest();
        }
    }
}
