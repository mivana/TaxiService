using RentApp.Models.Entities;
using RentApp.Persistance.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentApp.Persistance.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AppUser, int> AppUsers { get; set; }
        IRepository<Address, int> Addresses { get; set; }
        IRepository<Car, int> Cars { get; set; }
        IRepository<Comment, int> Comments { get; set; }
        IRepository<Ride, int> Rides { get; set; }
        IRepository<Location, int> Locations { get; set; }
        int Complete();
    }
}
