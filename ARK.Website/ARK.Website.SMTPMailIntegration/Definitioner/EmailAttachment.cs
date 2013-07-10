using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class EmailAttachment
    {
        #region Properties
        internal byte[] Content { get; set; }
        internal string Name { get; set; }
        #endregion

        #region Methods
        internal void ValidateData()
        {
            if (Content == null || Content.Length == 0)
            {
                throw new Exception(typeof(EmailAttachment).Name + " must have Content");
            }
        }

        public override string ToString()
        {
            string returnValue = String.Empty;
            if (!String.IsNullOrEmpty(Name))
            {
                returnValue = Name + " ";
            }
            if (Content != null)
            {
                returnValue += "byte[" + Content.Length + "]";
            }
            return returnValue;
        }
        #endregion
    }
}
