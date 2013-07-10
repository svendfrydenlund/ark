using ARK.Website.Common.Enum;
using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.EntityFramework.Main
{
    public partial class Regnskabsmedlem: IJegHarIndsatTid, IJegHarOpdateretTid
    {
        public KoenEnum Koen
        {
            get
            {
                KoenEnum koen = KoenEnum.Undefined;
                if(!String.IsNullOrEmpty(KoenFelt))
                {
                    Enum.TryParse<KoenEnum>(KoenFelt, out koen);
                }
                return koen;
            }
            set
            {
                if (value == KoenEnum.Undefined)
                {
                    KoenFelt = null;
                }
                else
                {
                    KoenFelt = value.ToString();
                }
            }
        }

        public MedlemsstatusEnum Status
        {
            get
            {
                MedlemsstatusEnum status = MedlemsstatusEnum.Undefined;
                if (!String.IsNullOrEmpty(StatusFelt))
                {
                    Enum.TryParse<MedlemsstatusEnum>(StatusFelt, out status);
                }
                return status;
            }
            set
            {
                if (value == MedlemsstatusEnum.Undefined)
                {
                    StatusFelt = null;
                }
                else
                {
                    StatusFelt = value.ToString();
                }
            }
        }
    }
}
