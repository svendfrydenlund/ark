//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ARK.Website.EntityFramework.Main
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mail
    {
        public Mail()
        {
            this.MailStatistiks = new HashSet<MailStatistik>();
        }
    
        public int ID { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string Afsender { get; set; }
        public System.DateTime Afsendt { get; set; }
    
        public virtual ICollection<MailStatistik> MailStatistiks { get; set; }
    }
}
