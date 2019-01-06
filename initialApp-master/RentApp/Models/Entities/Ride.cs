using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public enum Status
    {
        Created = 0,
        Cancelled,
        Formed,
        Proccessed,
        Accepted,
        Failed,
        Successfull
    }

    public class Ride
    {
        public int Id { get; set; }

        public DateTime OrderDT { get; set; }
        public string CarType { get; set; }
        public double Price { get; set; }
        public Status Status { get; set; }
        public bool Deleted { get; set; }


        [Required]
        [ForeignKey("StartLocation")]
        public int StartLocationID { get; set; }
        public virtual Location StartLocation { get; set; }

        [ForeignKey("DestinationLocation")]
        public int? DestinationLocationID { get; set; }
        public virtual Location DestinationLocation { get; set; }

        [ForeignKey("Customer")]
        public int? AppUserID { get; set; }
        public virtual AppUser Customer { get; set; }

        [ForeignKey("Dispatcher")]
        public int? DispatcherID { get; set; }
        public virtual AppUser Dispatcher { get; set; }

        [ForeignKey("TaxiDriver")]
        public int? TaxiDriverID { get; set; }
        public virtual AppUser TaxiDriver { get; set; }

        [InverseProperty("Ride")]
        public virtual ICollection<Comment> UserComment { get; set; }

        public Ride()
        {
            UserComment = new HashSet<Comment>();
        }

    }
}