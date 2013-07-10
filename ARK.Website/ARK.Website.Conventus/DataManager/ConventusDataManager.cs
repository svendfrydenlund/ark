using ARK.Website.Common.DTO;
using ARK.Website.Common.Interface;
using ARK.Website.Conventus.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Conventus.DataManager
{
    public class ConventusDataManager : IRegnskabsmedlemsManager
    {
        private ConventusDAC _dac = new ConventusDAC();

        public List<RegnskabsmedlemDTO> HentRegnskabsmedlemmer()
        {
            return _dac.HentRegnskabsmedlemmer();
        }

        public RegnskabsmedlemDTO HentRegnskabsmedlem(int arkID)
        {
            return _dac.HentRegnskabsmedlem(arkID);
        }
    }
}
