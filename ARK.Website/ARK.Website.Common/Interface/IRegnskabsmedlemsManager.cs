using ARK.Website.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Interface
{
    public interface IRegnskabsmedlemsManager
    {
        List<RegnskabsmedlemDTO> HentRegnskabsmedlemmer();

        RegnskabsmedlemDTO HentRegnskabsmedlem(int arkID);
    }
}
