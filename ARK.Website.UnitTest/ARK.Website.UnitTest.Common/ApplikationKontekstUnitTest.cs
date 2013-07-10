using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.UnitTest.Common
{
    public class ApplikationKontekstUnitTest : IApplikationKontekst
    {
        public string GivID()
        {
            return "ARK.Website.UnitTest";
        }
    }
}
