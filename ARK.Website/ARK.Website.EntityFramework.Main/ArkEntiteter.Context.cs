﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ArkDatabase : DbContext
    {
        public ArkDatabase()
            : base("name=ArkDatabase")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Baad> Baads { get; set; }
        public DbSet<BaadKategori> BaadKategoris { get; set; }
        public DbSet<BaadType> BaadTypes { get; set; }
        public DbSet<Medlem> Medlems { get; set; }
        public DbSet<Tur> Turs { get; set; }
        public DbSet<Turdeltager> Turdeltagers { get; set; }
        public DbSet<Regnskabsmedlem> Regnskabsmedlems { get; set; }
        public DbSet<Rostatistik> Rostatistiks { get; set; }
        public DbSet<Begivenhed> Begivenheds { get; set; }
    }
}