using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ARK.Website.Common.Enum;
using ARK.Website.EntityFramework.Main;
using System.Collections.Generic;
using ARK.Website.BLL.BO;
using System.Linq;
using ARK.Website.Common.Manager;
using ARK.Website.UnitTest.Common;

namespace ARK.Website.BLL.UnitTest
{
    [TestClass]
    public class MedlemBOTest
    {
        [TestMethod]
        public void OpdaterRostatistikTest()
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

            MedlemBO medlemBO = new MedlemBO(medlem);
            medlemBO.OpdaterRostatistik();
            Assert.AreEqual(medlem.Rostatistik.KilometerDetteAar, turlaengderDetteAar.Sum());
            Assert.AreEqual(medlem.Rostatistik.KilometerSidsteAar, turlaengderSidsteAar.Sum());
        }
    }
}
