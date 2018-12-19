using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Address
    {
        
        public int Id { get; set; }
        public string StreetName { get; set; }
        public int Number { get; set; }
        public string Town { get; set; }
        public int AreaCode { get; set; }

        public bool Deleted { get; set; }
    }
}