﻿using Microsoft.AspNet.Identity.Owin;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/Car")]
    public class CarController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public CarController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public IEnumerable<Car> GetCars()
        {
            return unitOfWork.Cars.GetAll().Where(u => u.Deleted == false);
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Car))]
        public IHttpActionResult AddCar(Car car)
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            unitOfWork.Cars.Add(car);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Driver")]
        [ResponseType(typeof(Car))]
        public IHttpActionResult UpdateCar(int id, Car car)
        {
            var ccar = unitOfWork.Cars.FirstOrDefault(i => i.Id == id);
            if (ccar == null)
                return BadRequest("No car found");

            var result = unitOfWork.Cars.Find(c => c.TaxiNumber == car.TaxiNumber && c.Id.ToString() != ccar.Id.ToString()).ToList();
            if (result.Count() != 0)
            {
                return BadRequest("TaxiNumber not Unique");
            }

            ccar.RegistrationPlate = car.RegistrationPlate;
            ccar.TaxiNumber = car.TaxiNumber;
            ccar.YearMade = car.YearMade;
            ccar.CarType = car.CarType;

            try
            {
                unitOfWork.Cars.Update(ccar);
                unitOfWork.Complete();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest();
            }    
            
        }

    }
}
