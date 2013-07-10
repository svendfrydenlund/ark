using ARK.Website.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.UnitTest.Common
{
    public class LoggingManagerUnitTest : ILoggingManager
    {
        StringBuilder _sb = new StringBuilder();

        public void LogException(string noegleord, Exception exception)
        {
            _sb.AppendLine("### ERROR ###" + Environment.NewLine + noegleord + Environment.NewLine + exception.ToString() + Environment.NewLine);
        }

        public void LogBesked(string noegleord, string besked)
        {
            _sb.AppendLine("### Besked ###" + Environment.NewLine + noegleord + Environment.NewLine + besked + Environment.NewLine);
        }

        public void LogAdvarsel(string noegleord, string advarsel)
        {
            _sb.AppendLine("### ADVARSEL ###" + Environment.NewLine + noegleord + Environment.NewLine + advarsel + Environment.NewLine);
        }

        public void RethrowException(Exception exception)
        {
            throw new Exception("Rethrown", exception);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public void Clear()
        {
            _sb.Clear();
        }
    }
}
