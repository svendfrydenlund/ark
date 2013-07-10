using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.DTO.EMail
{
    public class EMailHtmlBodyIndlejretBilledeDTO
    {
        public string BilledeID { get; set; }

        public byte[] Data { get; set; }
    }
}
