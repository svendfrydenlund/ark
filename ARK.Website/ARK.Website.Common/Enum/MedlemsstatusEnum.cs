using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Enum
{
    public enum MedlemsstatusEnum: byte
    {
        Undefined = 0,
        IkkeAktiveret,
        Aktiv,
        Inaktiv,
        Gammel
    }
}
