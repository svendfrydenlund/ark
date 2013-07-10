using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class EmailUser
    {
        #region Properties
        internal string Name { get; set; }
        internal string EmailAddress { get; set; }
        #endregion

        #region Methods
        internal void ValidateData()
        {
            if (String.IsNullOrEmpty(EmailAddress))
            {
                throw new Exception("EmailUser must have email address");
            }
        }

        public override string ToString()
        {
            string returnValue = String.Empty;
            if (!String.IsNullOrEmpty(EmailAddress))
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    returnValue = Name + " <" + EmailAddress + ">";
                }
                else
                {
                    returnValue = EmailAddress;
                }
            }
            return returnValue;
        }
        #endregion
    }
}
