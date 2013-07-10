using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.Conventus.DataManager;
using ARK.Website.Common.DTO;
using System.Collections.Generic;
using ARK.Website.Common.Manager;
using ARK.Website.UnitTest.Common;

namespace ARK.Website.Conventus.UnitTest
{
    [TestClass]
    public class ConventusDataManagerTest
    {
        public ConventusDataManagerTest()
        {
            UnitTestHelper.InitierAlleKomponenterMedDefault();
            KomponentManager.RegnskabsmedlemsManager = new ConventusDataManager();
        }

        [TestMethod]
        public void HentRegnskabsmedlemmerTest()
        {
            LoggingManagerUnitTest loggingManager = new LoggingManagerUnitTest();
            KomponentManager.LoggingManager = loggingManager;

            List<RegnskabsmedlemDTO> medlemmer = KomponentManager.RegnskabsmedlemsManager.HentRegnskabsmedlemmer();
            string notater = loggingManager.ToString();
            
            Assert.AreNotEqual<int>(0, medlemmer.Count, notater);
        }

        [TestMethod]
        public void HentRegnskabsmedlemTest()
        {
            LoggingManagerUnitTest loggingManager = new LoggingManagerUnitTest();
            KomponentManager.LoggingManager = loggingManager;

            List<RegnskabsmedlemDTO> medlemmer = KomponentManager.RegnskabsmedlemsManager.HentRegnskabsmedlemmer();
            Assert.IsTrue(medlemmer.Count > 10);
            RegnskabsmedlemDTO medlemTilAtHente = medlemmer[10];
            int arkID = medlemTilAtHente.ArkID;

            loggingManager.Clear();

            RegnskabsmedlemDTO hentetMedlem = KomponentManager.RegnskabsmedlemsManager.HentRegnskabsmedlem(arkID);

            string notater = loggingManager.ToString();
            Assert.IsNotNull(hentetMedlem, notater);
            Assert.AreEqual<string>(hentetMedlem.Navn, medlemTilAtHente.Navn, notater);
        }
    }
}