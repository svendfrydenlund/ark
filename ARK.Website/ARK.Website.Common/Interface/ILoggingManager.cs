using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Interface
{
    public interface ILoggingManager
    {
        void LogException(string noegleord, Exception exception);

        void LogAdvarsel(string noegleord, string advarsel);

        void LogBesked(string noegleord, string besked);
    }
}
