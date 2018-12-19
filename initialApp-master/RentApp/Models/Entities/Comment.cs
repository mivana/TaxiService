using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public string Content { get; set; }
        public DateTime? DateCreated { get; set; }
        public int Rating { get; set; }
        public bool Deleted { get; set; }

        [Required]
        [ForeignKey("AppUser")]
        public int AppUserID { get; set; }
        public virtual AppUser AppUser { get; set; }

        [Required]
        [ForeignKey("Ride")]
        public int RideID { get; set; }
        public virtual Ride Ride { get; set; }

    }
}