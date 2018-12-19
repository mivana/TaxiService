using RentApp.Models.Entities;
using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Unity.Attributes;

namespace RentApp.Persistance.UnitOfWork
{
    public class DemoUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        [Dependency]
        public IRepository<AppUser, int> AppUsers { get; set; }

        [Dependency]
        public IRepository<Address, int> Addresses { get; set; }

        [Dependency]
        public IRepository<Car, int> Cars { get; set; }

        [Dependency]
        public IRepository<Comment, int> Comments { get; set; }

        [Dependency]
        public IRepository<Location, int> Locations { get; set; }

        [Dependency]
        public IRepository<Ride, int> Rides { get; set; }

        public DemoUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}