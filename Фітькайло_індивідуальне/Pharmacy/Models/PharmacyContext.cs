using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Pharmacy.Models
{
    public class PharmacyContext : DbContext
    {
        public PharmacyContext() : base("DefaultConnection")
        {
        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<User> Users { get; set; }


    }
}
