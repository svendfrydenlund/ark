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
    
    public partial class Medlem
    {
        public Medlem()
        {
            this.Turdeltagers = new HashSet<Turdeltager>();
        }
    
        public int ID { get; set; }
        public System.DateTime IndsatTid { get; set; }
        public System.DateTime OpdateretTid { get; set; }
        public int ArkID { get; set; }
        public string StatusFelt { get; set; }
        public string Navn { get; set; }
        public string Adresse { get; set; }
        public string AdressePostNummer { get; set; }
        public string AdresseBy { get; set; }
        public string EMailAdresse { get; set; }
        public Nullable<System.DateTime> Foedselsdato { get; set; }
        public string KoenFelt { get; set; }
        public string MobilNummer { get; set; }
        public int RostatistikID { get; set; }
    
        public virtual ICollection<Turdeltager> Turdeltagers { get; set; }
        public virtual Rostatistik Rostatistik { get; set; }
    }
}
