using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARK.Website.Common.Manager
{
    public static class CacheManager
    {
        private static Dictionary<string, object> _objectByIdentifier = new Dictionary<string, object>();
        private static bool _koereUnitTestOgBenytterStatiskCacheOgIkkeSession = false;

        public static bool KoereUnitTestOgBenytterStatiskCacheOgIkkeSession
        {
            get { return _koereUnitTestOgBenytterStatiskCacheOgIkkeSession; }
            set { _koereUnitTestOgBenytterStatiskCacheOgIkkeSession = value; }
        }

        public static void SetValue(string identifier, object value)
        {
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.SessionState.HttpSessionState session = System.Web.HttpContext.Current.Session;
                object existingValue = session[identifier];
                if (existingValue == null)
                {
                    session.Add(identifier, value);
                }
                else
                {
                    session[identifier] = value;
                }
            }
            else
            {
                if (!KoereUnitTestOgBenytterStatiskCacheOgIkkeSession)
                {
                    throw new NotSupportedException("System.Web.HttpContext.Current er NULL. Det må ikke ske, når vi ikke køre unit tests");
                }
                if (_objectByIdentifier.Keys.Contains(identifier))
                {
                    _objectByIdentifier[identifier] = value;
                }
                else
                {
                    _objectByIdentifier.Add(identifier, value);
                }
            }
        }

        public static T GetValue<T>(string identifier)
        {
            object value = null;
            if (System.Web.HttpContext.Current != null)
            {
                System.Web.SessionState.HttpSessionState session = System.Web.HttpContext.Current.Session;
                value = session[identifier];
            }
            else
            {
                if (!KoereUnitTestOgBenytterStatiskCacheOgIkkeSession)
                {
                    throw new NotSupportedException("System.Web.HttpContext.Current er NULL. Det må ikke ske, når vi ikke køre unit tests");
                }
                if (_objectByIdentifier.Keys.Contains(identifier))
                {
                    value = _objectByIdentifier[identifier];
                }
            }
            return (T)value;
        }
    }
}
