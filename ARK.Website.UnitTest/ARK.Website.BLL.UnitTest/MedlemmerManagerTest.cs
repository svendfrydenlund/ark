using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.BLL.BO;
using ARK.Website.Common.Manager;
using ARK.Website.UnitTest.Common;
using ARK.Website.EntityFramework.Main;
using System.Collections.Generic;
using System.Linq;
using ARK.Website.Conventus.DataManager;
using ARK.Website.BLL.Manager;
using ARK.Website.Common.Enum;
using ARK.Website.Common.DTO;

namespace ARK.Website.BLL.UnitTest
{
    [TestClass]
    public class MedlemmerManagerTest
    {
        #region Test af SynkroniserRegnskabsmedlemmerOgMedlemmer
        #region Test af medlemsopdateringer med stub til Regnskabsmedlemmer (RegnskabsmedlemsManagerUnitTest)
        #region Hjaelpemetoder
        private void InitierStubMedlemsopdatringerTest(out LoggingManagerUnitTest loggingManager, out RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager)
        {
            loggingManager = new LoggingManagerUnitTest();
            regnskabsmedlemsManager = new RegnskabsmedlemsManagerUnitTest();

            UnitTestHelper.InitierAlleKomponenterMedDefault();
            KomponentManager.LoggingManager = loggingManager;
            KomponentManager.RegnskabsmedlemsManager = regnskabsmedlemsManager;

            UnitTestHelper.SletAlt();
        }

        public static void IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager, int N, MedlemsstatusEnum? forceretRegnskabsmedlemStatus = null)
        {
            int antalNyeEntiteter = N;
            for (int i = 0; i < antalNyeEntiteter; i++)
            {
                regnskabsmedlemsManager.LavNytRegnskabsmedlem(forceretRegnskabsmedlemStatus);
            }
            (new MedlemmerManager()).SynkroniserRegnskabsmedlemmerOgMedlemmer(false);
        }

        private string Synkroniser_TestResultat_ReturnerNotater(
            int arkID,
            RegnskabsmedlemDTO regnskabsmedlem,
            LoggingManagerUnitTest loggingManager,
            bool medlemOgRegnskabsmeldemNavnSkalVaereEns,
            bool kendtRegnskabsmedlemOgRegnskabsmedlemNavnSkalVaereEns,
            MedlemsstatusEnum? medlemstatusHvisForskelligFraRegnskabsmedlemstatus = null)
        {
            loggingManager.Clear();
            (new MedlemmerManager()).SynkroniserRegnskabsmedlemmerOgMedlemmer(false);

            string notater = KomponentManager.LoggingManager.ToString();
            Assert.IsTrue(!String.IsNullOrEmpty(notater), notater);

            using (ArkDatabase db = new ArkDatabase())
            {
                Medlem medlem = db.Medlems.First(medlemItem => medlemItem.ArkID == arkID);
                Regnskabsmedlem kendtRegnskabsmedlem = db.Regnskabsmedlems.First(medlemItem => medlemItem.ArkID == arkID);

                if (medlemstatusHvisForskelligFraRegnskabsmedlemstatus.HasValue)
                {
                    Assert.AreEqual(medlemstatusHvisForskelligFraRegnskabsmedlemstatus.Value, medlem.Status);
                }
                else
                {
                    Assert.AreEqual(regnskabsmedlem.Status, medlem.Status);
                }

                if (medlemOgRegnskabsmeldemNavnSkalVaereEns)
                {
                    Assert.AreEqual(regnskabsmedlem.Navn, medlem.Navn);
                }
                else
                {
                    Assert.AreNotEqual(regnskabsmedlem.Navn, medlem.Navn);
                }

                Assert.AreEqual(regnskabsmedlem.Status, kendtRegnskabsmedlem.Status);
                if (kendtRegnskabsmedlemOgRegnskabsmedlemNavnSkalVaereEns)
                {
                    Assert.AreEqual(regnskabsmedlem.Navn, kendtRegnskabsmedlem.Navn);
                }
                else
                {
                    Assert.AreNotEqual(regnskabsmedlem.Navn, kendtRegnskabsmedlem.Navn);
                }
            }
            return notater;
        }
        #endregion

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_FoersteGang()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            LoggingManagerUnitTest loggingManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            InitierStubMedlemsopdatringerTest(out loggingManager, out regnskabsmedlemsManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter);

            string notater = loggingManager.ToString();
            Assert.IsTrue(!String.IsNullOrEmpty(notater), notater);

            using (ArkDatabase db = new ArkDatabase())
            {
                List<Medlem> medlemmer = db.Medlems.ToList();
                List<Regnskabsmedlem> kendteRegnskabsmedlemmer = db.Regnskabsmedlems.ToList();
                Assert.AreEqual(antalNyeEntiteter, medlemmer.Count);
                Assert.AreEqual(antalNyeEntiteter, kendteRegnskabsmedlemmer.Count);
            }
        }

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_MedlemAktivTilGammel()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            LoggingManagerUnitTest loggingManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            InitierStubMedlemsopdatringerTest(out loggingManager, out regnskabsmedlemsManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            string nytNavn = Guid.NewGuid().ToString();
            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();
            UnitTestHelper.OpdaterMedlemStatus(arkID, MedlemsstatusEnum.Aktiv);

            //Aendre regnskabsmedlem i stubben
            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemsManager.Regnskabsmedlemmer.First(medlemItem => medlemItem.ArkID == arkID);
            regnskabsmedlem.Status = MedlemsstatusEnum.Gammel;
            regnskabsmedlem.Navn = nytNavn;

            string notater = Synkroniser_TestResultat_ReturnerNotater(arkID, regnskabsmedlem, loggingManager, false, true);
        }

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_MedlemAktivTilInaktiv()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            LoggingManagerUnitTest loggingManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            InitierStubMedlemsopdatringerTest(out loggingManager, out regnskabsmedlemsManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            string nytNavn = Guid.NewGuid().ToString();
            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();
            UnitTestHelper.OpdaterMedlemStatus(arkID, MedlemsstatusEnum.Aktiv);

            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemsManager.Regnskabsmedlemmer.First(medlemItem => medlemItem.ArkID == arkID);
            regnskabsmedlem.Status = MedlemsstatusEnum.Inaktiv;
            regnskabsmedlem.Navn = nytNavn;

            string notater = Synkroniser_TestResultat_ReturnerNotater(arkID, regnskabsmedlem, loggingManager, false, false);
        }

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_MedlemGammelTilAktiv()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            LoggingManagerUnitTest loggingManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            InitierStubMedlemsopdatringerTest(out loggingManager, out regnskabsmedlemsManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Gammel);

            string nytNavn = Guid.NewGuid().ToString();
            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();

            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemsManager.Regnskabsmedlemmer.First(medlemItem => medlemItem.ArkID == arkID);
            regnskabsmedlem.Status = MedlemsstatusEnum.Aktiv;
            regnskabsmedlem.Navn = nytNavn;

            string notater = Synkroniser_TestResultat_ReturnerNotater(arkID, regnskabsmedlem, loggingManager, true, true, MedlemsstatusEnum.IkkeAktiveret);
        }

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_MedlemIkkeAktiveretTilInaktiv()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            LoggingManagerUnitTest loggingManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            InitierStubMedlemsopdatringerTest(out loggingManager, out regnskabsmedlemsManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            string nytNavn = Guid.NewGuid().ToString();
            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();

            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemsManager.Regnskabsmedlemmer.First(medlemItem => medlemItem.ArkID == arkID);
            regnskabsmedlem.Status = MedlemsstatusEnum.Inaktiv;
            regnskabsmedlem.Navn = nytNavn;

            string notater = Synkroniser_TestResultat_ReturnerNotater(arkID, regnskabsmedlem, loggingManager, true, true, MedlemsstatusEnum.IkkeAktiveret);
        }
        #endregion

        [TestMethod]
        public void SynkroniserRegnskabsmedlemmerOgMedlemmerTest_Conventus()
        {
            UnitTestHelper.InitierAlleKomponenterMedDefault();
            KomponentManager.RegnskabsmedlemsManager = new ConventusDataManager();
            KomponentManager.LoggingManager = new LoggingManager();

            MedlemmerManager medlemmer = new MedlemmerManager();
            medlemmer.SynkroniserRegnskabsmedlemmerOgMedlemmer();

            //See i database efter resultat
        }
        #endregion

        #region Test af TryLogMedlemInd
        #region Hjaelpemetoder
        private void InitierStubLogMedlemIndTest(out LoggingManagerUnitTest loggingManager, out RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager, out MedlemmerManager medlemmerManager)
        {
            loggingManager = new LoggingManagerUnitTest();
            regnskabsmedlemsManager = new RegnskabsmedlemsManagerUnitTest();
            medlemmerManager = new MedlemmerManager();

            UnitTestHelper.InitierAlleKomponenterMedDefault();
            KomponentManager.JegHarIndloggetMedlemIDOgArkID = medlemmerManager;
            KomponentManager.LoggingManager = loggingManager;
            KomponentManager.RegnskabsmedlemsManager = regnskabsmedlemsManager;

            UnitTestHelper.SletAlt();
        }

        private string TryLogMedlemInd_TestResultat_ReturnerNotater(
            int arkID,
            int ID,
            MedlemLogindStatusEnum logindResultat,
            MedlemmerManager medlemmerManager,
            LoggingManagerUnitTest loggingManager)
        {
            loggingManager.Clear();

            Assert.IsTrue(medlemmerManager.TryLogMedlemIndMedArkID(arkID) == logindResultat, "MedlemmerManager.TryLogMedlemInd");

            IndloggetMedlemBO indloggetMedlem = MedlemmerManager.IndloggetMedlem;
            if (logindResultat == MedlemLogindStatusEnum.Succes ||
                logindResultat == MedlemLogindStatusEnum.Aktivering)
            {
                Assert.IsNotNull(indloggetMedlem, "MedlemmerManager.IndloggetMedlem");

                int? indloggetMedlemID = KomponentManager.JegHarIndloggetMedlemIDOgArkID.IndloggetMedlemID;
                Assert.IsNotNull(indloggetMedlemID, "KomponentManager.JegHarIndloggetMedlemID.IndloggetMedlemID");
                Assert.AreEqual(ID, indloggetMedlemID.Value);

                int? indloggetMedlemArkID = KomponentManager.JegHarIndloggetMedlemIDOgArkID.IndloggetMedlemArkID;
                Assert.IsNotNull(indloggetMedlemArkID, "KomponentManager.JegHarIndloggetMedlemID.IndloggetMedlemArkID");
                Assert.AreEqual(arkID, indloggetMedlemArkID.Value);
            }
            else
            {
                Assert.IsNull(indloggetMedlem, "MedlemmerManager.IndloggetMedlem");

                int? indloggetMedlemID = KomponentManager.JegHarIndloggetMedlemIDOgArkID.IndloggetMedlemID;
                Assert.IsNull(indloggetMedlemID, "KomponentManager.JegHarIndloggetMedlemID.IndloggetMedlemID");

                int? indloggetMedlemArkID = KomponentManager.JegHarIndloggetMedlemIDOgArkID.IndloggetMedlemArkID;
                Assert.IsNull(indloggetMedlemArkID, "KomponentManager.JegHarIndloggetMedlemID.IndloggetMedlemArkID");
            }

            string notater = loggingManager.ToString();
            return notater;
        }
        #endregion

        [TestMethod]
        public void TryLogMedlemIndTest_MedlemEksistererMedStatusAktiv()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            MedlemmerManager medlemmerManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            LoggingManagerUnitTest loggingManager = null;
            InitierStubLogMedlemIndTest(out loggingManager, out regnskabsmedlemsManager, out medlemmerManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();
            UnitTestHelper.OpdaterMedlemStatus(arkID, MedlemsstatusEnum.Aktiv);
            Medlem medlemViProeverAtLoggeInd = null;
            using (ArkDatabase db = new ArkDatabase())
            {
                medlemViProeverAtLoggeInd = db.Medlems.First(medlemItem => medlemItem.ArkID == arkID);
            }
            int medlemID = medlemViProeverAtLoggeInd.ID;

            string noter = TryLogMedlemInd_TestResultat_ReturnerNotater(arkID, medlemID, MedlemLogindStatusEnum.Succes, medlemmerManager, loggingManager);
        }

        [TestMethod]
        public void TryLogMedlemIndTest_MedlemEksistererMedStatusIkkeAktiveret()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            MedlemmerManager medlemmerManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            LoggingManagerUnitTest loggingManager = null;
            InitierStubLogMedlemIndTest(out loggingManager, out regnskabsmedlemsManager, out medlemmerManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();
            UnitTestHelper.OpdaterMedlemStatus(arkID, MedlemsstatusEnum.IkkeAktiveret);
            Medlem medlemViProeverAtLoggeInd = null;
            using (ArkDatabase db = new ArkDatabase())
            {
                medlemViProeverAtLoggeInd = db.Medlems.First(medlemItem => medlemItem.ArkID == arkID);
            }
            int medlemID = medlemViProeverAtLoggeInd.ID;

            string noter = TryLogMedlemInd_TestResultat_ReturnerNotater(arkID, medlemID, MedlemLogindStatusEnum.Aktivering, medlemmerManager, loggingManager);
        }

        [TestMethod]
        public void TryLogMedlemIndTest_MedlemIkkeEksisterende()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            MedlemmerManager medlemmerManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            LoggingManagerUnitTest loggingManager = null;
            InitierStubLogMedlemIndTest(out loggingManager, out regnskabsmedlemsManager, out medlemmerManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            int arkID = -9999;

            string noter = TryLogMedlemInd_TestResultat_ReturnerNotater(arkID, -1, MedlemLogindStatusEnum.UkendtMedlem, medlemmerManager, loggingManager);
        }

        [TestMethod]
        public void TryLogMedlemIndTest_MedlemEksistererMedStatusAktivRegnskabsmedlemHarStatusGammel()
        {
            //NB: DENNE TEST SLETTER Medlem, Regnskabsmedlem og Begivenhed data
            MedlemmerManager medlemmerManager = null;
            RegnskabsmedlemsManagerUnitTest regnskabsmedlemsManager = null;
            LoggingManagerUnitTest loggingManager = null;
            InitierStubLogMedlemIndTest(out loggingManager, out regnskabsmedlemsManager, out medlemmerManager);

            int antalNyeEntiteter = 5;
            IndsaetNRegnskabsmedlemmerIRegnskabsmedlemsManagerOgSynkroniser(regnskabsmedlemsManager, antalNyeEntiteter, MedlemsstatusEnum.Aktiv);

            int arkID = UnitTestHelper.HentFoersteMedlemsArkID();
            UnitTestHelper.OpdaterMedlemStatus(arkID, MedlemsstatusEnum.Aktiv);
            Medlem medlemViProeverAtLoggeInd = null;
            using (ArkDatabase db = new ArkDatabase())
            {
                medlemViProeverAtLoggeInd = db.Medlems.First(medlemItem => medlemItem.ArkID == arkID);
            }
            int medlemID = medlemViProeverAtLoggeInd.ID;

            RegnskabsmedlemDTO regnskabsmedlem = regnskabsmedlemsManager.Regnskabsmedlemmer.First(medlemItem => medlemItem.ArkID == arkID);
            regnskabsmedlem.Status = MedlemsstatusEnum.Gammel;

            string noter = TryLogMedlemInd_TestResultat_ReturnerNotater(arkID, medlemID, MedlemLogindStatusEnum.RegnskabsmedlemstatusGammel, medlemmerManager, loggingManager);
        }
        #endregion
    }
}
