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
    
    public partial class Rostatistik
    {
        public Rostatistik()
        {
            this.Medlems = new HashSet<Medlem>();
        }
    
        public int ID { get; set; }
        public int KilometerDetteAar { get; set; }
        public int KilometerSidsteAar { get; set; }
    
        public virtual ICollection<Medlem> Medlems { get; set; }
    }
}
