using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class AppUser
    {
        public enum UserRole
        {
            AppUser = 0,
            Driver = 1,
            Admin = 2
        }

        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string JMBG { get; set; }
        public string ContactNumber { get; set; }
        public string Gender { get; set; }
        public bool DriverFree { get; set; }
        public bool Deleted { get; set; }
        public bool Blocked { get; set; }
        public UserRole Role { get; set; }

        [InverseProperty("Customer")]
        public virtual ICollection<Ride> CustomerRides { get; set; }

        [InverseProperty("Dispatcher")]
        public virtual ICollection<Ride> DispatcherRides { get; set; }

        [InverseProperty("TaxiDriver")]
        public virtual ICollection<Ride> DriverRides { get; set; }

        public int? DriverLocationId { get; set; }
        [ForeignKey("DriverLocationId")]
        public Location DriverLocation { get; set; }

        [InverseProperty("Driver")]
        public virtual ICollection<Car> DriverCars { get; set; }

        [InverseProperty("AppUser")]
        public virtual ICollection<Comment> UserComments { get; set; }
    }
}