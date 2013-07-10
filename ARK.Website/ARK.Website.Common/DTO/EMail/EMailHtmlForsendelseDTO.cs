using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.DTO.EMail
{
    public class EMailHtmlForsendelseDTO
    {
        public EMailHtmlForsendelseDTO()
        {
            To = new List<EMailBrugerDTO>();
            Cc = new List<EMailBrugerDTO>();
            Bcc = new List<EMailBrugerDTO>();
            Attachments = new List<EMailAttachmentDTO>();
            Body = new EMailHtmlBodyDTO();
            Sender = new EMailBrugerDTO();
        }

        public List<EMailBrugerDTO> To { get; set; }

        public List<EMailBrugerDTO> Cc { get; set; }

        public List<EMailBrugerDTO> Bcc { get; set; }

        public List<EMailAttachmentDTO> Attachments { get; set; }

        public string Subject { get; set; }

        public EMailHtmlBodyDTO Body { get; set; }

        public EMailBrugerDTO Sender { get; set; }
    }
}
