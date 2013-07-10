using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.DTO.EMail
{
    public class EMailAttachmentDTO
    {
        public byte[] Data { get; set; }

        public string Navn{ get; set; }
    }
}
