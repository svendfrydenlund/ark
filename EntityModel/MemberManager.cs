using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dk.arok.EntityModel
{
    public class MedlemContext : DbContext
    {
        public DbSet<Medlem> Medlemmer { get; set; }
        public DbSet<Baad> Baade { get; set; }
    }
}
