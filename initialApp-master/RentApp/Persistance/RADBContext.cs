using Microsoft.AspNet.Identity.EntityFramework;
using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance
{
    public class RADBContext : IdentityDbContext<RAIdentityUser>
    {
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Ride> Rides { get; set; }

        public RADBContext() : base("name=TaxiService")
        {
        }

        public static RADBContext Create()
        {
            return new RADBContext();
        }
    }
}