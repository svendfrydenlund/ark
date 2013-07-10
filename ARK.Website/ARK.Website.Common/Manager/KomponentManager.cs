using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Manager
{
    public static class KomponentManager
    {
        private static ILoggingManager _loggingManager = null;
        private static IRegnskabsmedlemsManager _regnskabsmedlemsManager = null;
        private static IJegHarInloggetMedlemIDogArkID _jegHarIndloggetMedlemIDOgArkID = null;
        private static IApplikationKontekst _applikationKontekst = null;
        private static IEMailDistributoer _eMailDistributoer = null;

        public static ILoggingManager LoggingManager
        {
            get
            {
                if (_loggingManager == null)
                {
                    throw new Exception("KomponentManager.LoggingManager ikke givet");
                }
                return _loggingManager;
            }
            set
            {
                _loggingManager = value;
            }
        }

        public static IRegnskabsmedlemsManager RegnskabsmedlemsManager
        {
            get
            {
                if (_regnskabsmedlemsManager == null)
                {
                    throw new Exception("KomponentManager.RegnskabsmedlemsManager ikke givet");
                }
                return _regnskabsmedlemsManager;
            }
            set
            {
                _regnskabsmedlemsManager = value;
            }
        }

        public static IJegHarInloggetMedlemIDogArkID JegHarIndloggetMedlemIDOgArkID
        {
            get
            {
                if (_jegHarIndloggetMedlemIDOgArkID == null)
                {
                    throw new Exception("KomponentManager.JegHarInloggetMedlemID ikke givet");
                }
                return _jegHarIndloggetMedlemIDOgArkID;
            }
            set
            {
                _jegHarIndloggetMedlemIDOgArkID = value;
            }
        }

        public static IApplikationKontekst ApplikationKontekst
        {
            get
            {
                if (_applikationKontekst == null)
                {
                    throw new Exception("KomponentManager.ApplikationKontekst ikke givet");
                }
                return _applikationKontekst;
            }
            set
            {
                _applikationKontekst = value;
            }
        }

        public static IEMailDistributoer EMailDistributoer
        {
            get
            {
                if (_eMailDistributoer == null)
                {
                    throw new Exception("KomponentManager.EMailDistributoer ikke givet");
                }
                return _eMailDistributoer;
            }
            set
            {
                _eMailDistributoer = value;
            }
        }
    }
}
