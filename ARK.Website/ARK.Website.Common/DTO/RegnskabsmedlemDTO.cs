using ARK.Website.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.DTO
{
    public class RegnskabsmedlemDTO
    {
        public int ArkID = -1;
        public MedlemsstatusEnum Status = MedlemsstatusEnum.Undefined;
        public string Navn = null;
        public string Adresse = null;
        public string AdressePostNummer = null;
        public string AdresseBy = null;
        public string EMailAdresse = null;
        public DateTime? Foedselsdato = null;
        public KoenEnum Koen = KoenEnum.Undefined;
        public string MobilNummer = null;
    }
}
