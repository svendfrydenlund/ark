using ARK.Website.Common.DTO.EMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Interface
{
    public interface IEMailDistributoer
    {
        void SendEMail(EMailHtmlForsendelseDTO eMailDefinition);
    }
}
