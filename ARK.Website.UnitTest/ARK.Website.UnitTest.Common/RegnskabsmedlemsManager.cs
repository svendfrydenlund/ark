using ARK.Website.Common.DTO;
using ARK.Website.Common.Enum;
using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.UnitTest.Common
{
    public class RegnskabsmedlemsManagerUnitTest: IRegnskabsmedlemsManager
    {
        private int _regnskabsmedlemNummer = 1;

        public RegnskabsmedlemsManagerUnitTest()
        {
            Regnskabsmedlemmer = new List<RegnskabsmedlemDTO>();
        }

        public List<RegnskabsmedlemDTO> Regnskabsmedlemmer
        {
            get;
            private set;
        }

        public RegnskabsmedlemDTO LavNytRegnskabsmedlem(MedlemsstatusEnum? forceretStatus = null)
        {
            RegnskabsmedlemDTO medlem = new RegnskabsmedlemDTO()
            {
                Adresse = "Adresse" + _regnskabsmedlemNummer,
                AdresseBy = "By" + _regnskabsmedlemNummer,
                AdressePostNummer = _regnskabsmedlemNummer.ToString(),
                ArkID = _regnskabsmedlemNummer,
                EMailAdresse = "EMail" + _regnskabsmedlemNummer,
                Foedselsdato = new DateTime(1977 + _regnskabsmedlemNummer, 1, 1),
                Koen = (_regnskabsmedlemNummer % 2 == 0 ? KoenEnum.Mand : KoenEnum.Kvinde),
                MobilNummer = (22220000 + _regnskabsmedlemNummer).ToString(),
                Navn = "Navn" + _regnskabsmedlemNummer,
                Status = (forceretStatus.HasValue ? forceretStatus.Value : (MedlemsstatusEnum)((_regnskabsmedlemNummer % 3) + 2))
            };
            _regnskabsmedlemNummer++;
            Regnskabsmedlemmer.Add(medlem);
            return medlem;
        }

        public List<RegnskabsmedlemDTO> HentRegnskabsmedlemmer()
        {
            return Regnskabsmedlemmer;
        }


        public RegnskabsmedlemDTO HentRegnskabsmedlem(int arkID)
        {
            return Regnskabsmedlemmer.FirstOrDefault(medlemItem => medlemItem.ArkID == arkID);
        }
    }
}
