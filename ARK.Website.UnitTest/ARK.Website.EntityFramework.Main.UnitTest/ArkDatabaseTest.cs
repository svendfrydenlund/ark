using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.Common.Manager;
using ARK.Website.UnitTest.Common;
using System.Linq;
using System.Collections.Generic;
using ARK.Website.Common.Enum;

namespace ARK.Website.EntityFramework.Main.UnitTest
{
    [TestClass]
    public class ArkDatabaseTest
    {
        [TestMethod]
        public void ArkDatabase_DisposeAndModel()
        {
            //NB: DENNE TEST SLETTER ALT DATA I BASEN

            UnitTestHelper.InitierAlleKomponenterMedDefault();

            UnitTestHelper.SletAlt();

            Baad baad = UnitTestHelper.LavEnkeltBaad();
            UnitTestHelper.IndsaetXNytMedlem(1, MedlemsstatusEnum.Aktiv);
            Medlem medlem = UnitTestHelper.HentFoersteMedlem();
            DateTime detteAar = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            DateTime sidsteAar = detteAar.Subtract(TimeSpan.FromDays(365));
            List<int> turlaengderDetteAar = new List<int>();
            List<int> turlaengderSidsteAar = new List<int>();
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                int turlaengde = r.Next(100);
                turlaengderDetteAar.Add(turlaengde);
                UnitTestHelper.OpretTurPaaBaadOgMedlem(medlem.ID, baad.ID, detteAar, turlaengde);
                turlaengde = r.Next(100);
                turlaengderSidsteAar.Add(turlaengde);
                UnitTestHelper.OpretTurPaaBaadOgMedlem(medlem.ID, baad.ID, sidsteAar, turlaengde);
            }

            ArkDatabase db = null;
            #region Eager loading
            using (db = new ArkDatabase())
            {
                //Eager loading. Inkluderer fuld load af de nævnte felter og lister
                medlem = db.Medlems.Include("Rostatistik").Include("Turdeltagers.Tur").First(medlemItem => medlemItem.ID == medlem.ID);
            }
            bool fejler = false;
            try
            {
                Rostatistik rostatistik = medlem.Rostatistik;
                List<Turdeltager> turdeltagere = medlem.Turdeltagers.ToList();
                List<Tur> ture = turdeltagere.Select(turdeltagerItem => turdeltagerItem.Tur).ToList();
            }
            catch (Exception)
            {
                fejler = true;
            }
            Assert.IsFalse(fejler);
            #endregion

            #region Lazy loading - Allerede lazy loadede Properties forbliver efter disposet forbindelse
            using (db = new ArkDatabase())
            {
                //Lazy loading. Loader først felter og lister ved forespørgsel
                medlem = db.Medlems.First(medlemItem => medlemItem.ID == medlem.ID);
                Rostatistik rostatistik = medlem.Rostatistik;
                List<Turdeltager> turdeltagere = medlem.Turdeltagers.ToList();
                List<Tur> ture = turdeltagere.Select(turdeltagerItem => turdeltagerItem.Tur).ToList();
            }
            fejler = false;
            try
            {
                Rostatistik rostatistik = medlem.Rostatistik;
                List<Turdeltager> turdeltagere = medlem.Turdeltagers.ToList();
                List<Tur> ture = turdeltagere.Select(turdeltagerItem => turdeltagerItem.Tur).ToList();
            }
            catch (Exception)
            {
                fejler = true;
            }
            Assert.IsFalse(fejler);
            #endregion

            #region Lazy loading - Tilgang af ikke loadede Properties efter disposed Arkdatabase -> Disposed exception
            using (db = new ArkDatabase())
            {
                medlem = db.Medlems.First(medlemItem => medlemItem.ID == medlem.ID);
            }
            fejler = false;
            try
            {
                Rostatistik rostatistik = medlem.Rostatistik;
                List<Turdeltager> turdeltagere = medlem.Turdeltagers.ToList();
                List<Tur> ture = turdeltagere.Select(turdeltagerItem => turdeltagerItem.Tur).ToList();
            }
            catch (Exception)
            {
                fejler = true;
            }
            Assert.IsTrue(fejler);
            #endregion
        }
    }
}
