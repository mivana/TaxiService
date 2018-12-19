using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Location
    {
        public int Id { get; set; }

        public double XPos { get; set; }
        public double YPos { get; set; }
        public bool Deleted { get; set; }

        [Required]
        [ForeignKey("Address")]
        public int AddressID { get; set; }
        public virtual Address Address { get; set; }

    }
}