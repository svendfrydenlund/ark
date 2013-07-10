using ARK.Website.BLL.Manager;
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
    public class IndloggetMedlemBO : MedlemBO
    {
        #region Private felter
        private bool _erRegnskabsdataOpdateret = false;
        private MedlemLogindStatusEnum _logindStatus = MedlemLogindStatusEnum.Undefined;
        private Regnskabsmedlem _kendtRegnskabsmedlem = null;
        private RegnskabsmedlemDTO _regnskabsmedlem = null;
        #endregion

        #region Konstruktion
        internal IndloggetMedlemBO(int arkID)
        {
            using (ARK.Website.EntityFramework.Main.ArkDatabase db = new EntityFramework.Main.ArkDatabase())
            {
                Data = db.Medlems.FirstOrDefault(medlemItem => medlemItem.ArkID == arkID);
                if (Data == null)
                {
                    LogindStatus = MedlemLogindStatusEnum.UkendtMedlem;
                }
                else
                {
                    KendtRegnskabsmedlem = db.Regnskabsmedlems.First(medlemItem => medlemItem.ArkID == arkID);
                    Regnskabsmedlem = KomponentManager.RegnskabsmedlemsManager.HentRegnskabsmedlem(arkID);
                    if (Regnskabsmedlem == null ||
                        Regnskabsmedlem.Status == MedlemsstatusEnum.Gammel)
                    {
                        LogindStatus = MedlemLogindStatusEnum.RegnskabsmedlemstatusGammel;
                    }
                    else
                    {
                        switch (Data.Status)
                        {
                            case MedlemsstatusEnum.Inaktiv:
                            case MedlemsstatusEnum.Aktiv:
                                {
                                    ErRegnskabsdataOpdateret = MedlemmerManager.HarRegnskabsmedlemOgKendtRegnskabsmedlemSammeData(Regnskabsmedlem, KendtRegnskabsmedlem);
                                    LogindStatus = MedlemLogindStatusEnum.Succes;
                                }break;
                            case MedlemsstatusEnum.Gammel:
                            case MedlemsstatusEnum.IkkeAktiveret:
                                {
                                    MedlemmerManager.OverskrivMedRegnskabsmedlemsdata(Regnskabsmedlem, KendtRegnskabsmedlem);
                                    MedlemmerManager.OverskrivMedRegnskabsmedlemsdata(Regnskabsmedlem, Data);
                                    Data.Status = Regnskabsmedlem.Status;
                                    ErRegnskabsdataOpdateret = true;
                                    LogindStatus = MedlemLogindStatusEnum.Aktivering;
                                }break;
                            default:
                                {
                                    throw new NotImplementedException("Data.Status = " + Data.Status);
                                }
                        }
                    }
                }
            }
        }
        #endregion

        #region Properties
        public bool ErRegnskabsdataOpdateret
        {
            get { return _erRegnskabsdataOpdateret; }
            private set { _erRegnskabsdataOpdateret = value; }
        }

        public MedlemLogindStatusEnum LogindStatus
        {
            get { return _logindStatus; }
            private set { _logindStatus = value; }
        }

        public Regnskabsmedlem KendtRegnskabsmedlem
        {
            get { return _kendtRegnskabsmedlem; }
            private set { _kendtRegnskabsmedlem = value; }
        }

        public RegnskabsmedlemDTO Regnskabsmedlem
        {
            get { return _regnskabsmedlem; }
            private set { _regnskabsmedlem = value; }
        }
        #endregion
    }
}
