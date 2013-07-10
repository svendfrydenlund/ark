using ARK.Website.Common.DTO;
using ARK.Website.Common.Enum;
using ARK.Website.Common.Manager;
using ARK.Website.EntityFramework.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.BLL.BO
{
    public class MedlemBO
    {
        #region Private felter
        private Medlem _data = null;
        #endregion

        #region Konstruktion
        protected MedlemBO()
        {

        }

        public MedlemBO(Medlem data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            _data = data;
        }
        #endregion

        #region Properties
        public Medlem Data
        {
            get { return _data; }
            protected set { _data = value; }
        }
        #endregion

        #region Metoder
        public void OpdaterRostatistik()
        {
            using (ARK.Website.EntityFramework.Main.ArkDatabase db = new EntityFramework.Main.ArkDatabase())
            {
                DateTime now = DateTime.UtcNow;
                DateTime startSidsteAar = new DateTime(now.Year-1,1,1);
                DateTime startDetteAar = new DateTime(now.Year,1,1);
                db.Medlems.Attach(Data);
                db.Rostatistiks.Attach(Data.Rostatistik);

                var tureDetteAar =
                    db.Turs
                    .Where(turItem =>
                        turItem.StartTidspunkt > startDetteAar &&
                        db.Turdeltagers
                        .Where(turdeltagerItem => turdeltagerItem.MedlemID == Data.ID)
                        .Select(turdeltagerItem => turdeltagerItem.TurID).Contains(turItem.ID));

                Data.Rostatistik.KilometerDetteAar = 0;
                if (tureDetteAar.Count() > 0)
                {
                    Data.Rostatistik.KilometerDetteAar = tureDetteAar.Sum(turItem => turItem.AntalKilometer);
                }

                var tureSidsteAar =
                    db.Turs
                    .Where(turItem =>
                        turItem.StartTidspunkt > startSidsteAar &&
                        turItem.StartTidspunkt < startDetteAar &&
                        db.Turdeltagers
                        .Where(turdeltagerItem => turdeltagerItem.MedlemID == Data.ID)
                        .Select(turdeltagerItem => turdeltagerItem.TurID).Contains(turItem.ID));
                Data.Rostatistik.KilometerSidsteAar = 0;
                if (tureSidsteAar.Count() > 0)
                {
                    Data.Rostatistik.KilometerSidsteAar = tureSidsteAar.Sum(turItem => turItem.AntalKilometer);
                }
                db.SaveChanges();
            }
        }
        #endregion
    }
}
