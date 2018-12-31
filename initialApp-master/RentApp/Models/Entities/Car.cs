using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public enum CarType
    {
        Standard = 0,
        Combi = 1
    }

    public class Car
    {
        public int Id { get; set; }
        public string TaxiNumber { get; set; }
        public DateTime? YearMade { get; set; }
        public string RegistrationPlate { get; set; }
        public CarType CarType { get; set; }
        public bool Deleted { get; set; }

        [Required]
        [ForeignKey("Driver")]
        public int AppUserID { get; set; }
        public virtual AppUser Driver { get; set; }

    }
}