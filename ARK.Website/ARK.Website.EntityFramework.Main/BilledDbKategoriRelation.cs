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
    
    public partial class BilledDbKategoriRelation
    {
        public int billedeKategoriID { get; set; }
        public int illustrationID { get; set; }
        public int kategoriID { get; set; }
    
        public virtual BilledDbBilleder BilledDbBilleder { get; set; }
        public virtual BilledDbKkategorier BilledDbKkategorier { get; set; }
    }
}
