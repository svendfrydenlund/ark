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
    public class LoggingManager : ILoggingManager
    {
        #region Private metoder
        private void Log(string noegleord, string niveau, string beskrivelse)
        {
            Begivenhed begivenhed = new Begivenhed();
            begivenhed.Niveau = niveau;
            begivenhed.MedlemID = KomponentManager.JegHarIndloggetMedlemIDOgArkID.IndloggetMedlemID;
            begivenhed.Applikationskontekst = KomponentManager.ApplikationKontekst.GivID();
            begivenhed.Beskrivelse = beskrivelse;
            begivenhed.Noegleord = noegleord;

            using (ARK.Website.EntityFramework.Main.ArkDatabase db = new EntityFramework.Main.ArkDatabase())
            {
                db.Begivenheds.Add(begivenhed);
                db.SaveChanges();
            }
        }
        #endregion

        #region Metoder
        public void LogException(string noegleord, Exception exception)
        {
            Log(noegleord, "Fejl", exception.ToString());
        }

        public void LogAdvarsel(string noegleord, string advarsel)
        {
            Log(noegleord, "Advarsel", advarsel);
        }

        public void LogBesked(string noegleord, string besked)
        {
            Log(noegleord, "Besked", besked);
        }
        #endregion
    }
}
