using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.Common.Manager;
using ARK.Website.UnitTest.Common;
using ARK.Website.BLL.Manager;
using System.Reflection;
using ARK.Website.EntityFramework.Main;
using System.Linq;

namespace ARK.Website.BLL.UnitTest
{
    [TestClass]
    public class LoggingManagerTest
    {
        public LoggingManagerTest()
        {
            UnitTestHelper.InitierAlleKomponenterMedDefault();
            KomponentManager.LoggingManager = new LoggingManager();
        }

        [TestMethod]
        public void LogExceptionTest()
        {
            string exceptionBesked = MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + ": " + Guid.NewGuid().ToString();
            Log(LogException, "Fejl", MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, exceptionBesked);
        }

        [TestMethod]
        public void LogAdvarselTest()
        {
            string advarsel = MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + ": " + Guid.NewGuid().ToString();
            Log(KomponentManager.LoggingManager.LogAdvarsel, "Advarsel", MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, advarsel);
        }

        [TestMethod]
        public void LogBeskedTest()
        {
            string besked = MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name + ": " + Guid.NewGuid().ToString();
            Log(KomponentManager.LoggingManager.LogBesked, "Besked", MethodInfo.GetCurrentMethod().DeclaringType.Name + "." + MethodInfo.GetCurrentMethod().Name, besked);
        }

        private void LogException(string noegleord, string besked)
        {
            try
            {
                throw new Exception(besked);
            }
            catch (Exception exception)
            {
                KomponentManager.LoggingManager.LogException(noegleord, exception);
            }
        }

        private void Log(Action<string,string> action, string niveau, string noegleord, string besked)
        {
            using (ArkDatabase db = new ArkDatabase())
            {
                int antalBegivenhederFoer = db.Begivenheds.Count();
                action.Invoke(noegleord, besked);
                int antalBegivenhederEfter = db.Begivenheds.Count();
                Assert.AreEqual(antalBegivenhederEfter, antalBegivenhederFoer + 1);
                int senesteBegivenhedID = db.Begivenheds.Max(begivenhedItem => begivenhedItem.ID);
                Begivenhed senesteBegivenhed = db.Begivenheds.FirstOrDefault(begivenhedItem => begivenhedItem.ID == senesteBegivenhedID);
                Assert.IsNotNull(senesteBegivenhed);
                Assert.IsTrue(senesteBegivenhed.Beskrivelse.Contains(besked));
                Assert.AreEqual(senesteBegivenhed.Niveau, niveau);
            }
        }
    }
}
