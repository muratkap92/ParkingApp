using ParkingApp.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp.Core
{
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext()
        {
            Database.Connection.ConnectionString = "Server=.;Database=ParkingApp;Trusted_Connection=True;";
        }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Control> Controls { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
