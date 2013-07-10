using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Interface
{
    public interface IJegHarInloggetMedlemIDogArkID
    {
        int? IndloggetMedlemID
        {
            get;
        }

        int? IndloggetMedlemArkID
        {
            get;
        }
    }
}
