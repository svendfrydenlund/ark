using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARK.Website.SMTPMailIntegration.Definitioner
{
    internal class EmailBodyHTMLEmbeddedImage
    {
        #region Properties
        internal string BodyImageSourceID { get; set; }

        internal byte[] Content { get; set; }
        #endregion

        #region Methods
        internal void ValidateData()
        {
            if (Content == null || Content.Length == 0)
            {
                throw new Exception(typeof(EmailBodyHTMLEmbeddedImage).Name + ": Content null or empty");
            }
        }

        public override string ToString()
        {
            string returnValue = String.Empty;
            if (!String.IsNullOrEmpty(BodyImageSourceID))
            {
                returnValue = BodyImageSourceID + " ";
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
