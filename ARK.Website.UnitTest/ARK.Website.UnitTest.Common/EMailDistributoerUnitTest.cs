using ARK.Website.Common.DTO.EMail;
using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.UnitTest.Common
{
    public class EMailDistributoerUnitTest: IEMailDistributoer
    {
        public EMailDistributoerUnitTest()
        {
            ForsoegtSendteMails = new List<EMailHtmlForsendelseDTO>();
        }

        public void SendEMail(EMailHtmlForsendelseDTO eMailDefinition)
        {
            ForsoegtSendteMails.Add(eMailDefinition);
        }

        public List<EMailHtmlForsendelseDTO> ForsoegtSendteMails
        {
            get;
            private set;
        }
    }
}
