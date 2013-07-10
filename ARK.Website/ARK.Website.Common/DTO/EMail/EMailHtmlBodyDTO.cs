using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.DTO.EMail
{
    public class EMailHtmlBodyDTO
    {
        public EMailHtmlBodyDTO()
        {
            IndlejretBilleder = new List<EMailHtmlBodyIndlejretBilledeDTO>();
        }

        public string BodyTekst { get; set; }

        public List<EMailHtmlBodyIndlejretBilledeDTO> IndlejretBilleder { get; set; }
    }
}
