using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RentApp.Controllers
{
    [RoutePrefix("api/Ride")]
    public class RideController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public RideController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/Vehicle
        public IEnumerable<Ride> GetRides()
        {
            return unitOfWork.Rides.GetAll().Where(u => u.Deleted == false);
        }

        [HttpPost]
        [Authorize(Roles = "AppUser,Admin")]
        [Route("PostNewRide")]
        public IHttpActionResult PostNewRide(RideBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            CarType carType;

            if (model.CarType == "Standard")
            {
                carType = CarType.Standard;
            }
            else
            {
                carType = CarType.Combi;
            }

            var currentUser = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);

            Address address = new Address() { StreetName = model.StreetName, Number = model.Number, Town = model.Town, AreaCode = model.AreaCode, Deleted = false };

            try
            {
                unitOfWork.Addresses.Add(address);
            }
            catch(Exception e)
            {
                return BadRequest("Error while trying to add address to database");
            }
            var a = address;

            Location location = new Location() { XPos = 0, YPos = 0, Address = a, AddressID = a.Id, Deleted = false};

            try
            {
                unitOfWork.Locations.Add(location);
            }
            catch(Exception e)
            {
                return BadRequest("Error while trying to add location to database");
            }

            Ride newRide;

            if (currentUser.Role == AppUser.UserRole.AppUser)
            {
                newRide = new Ride() { CarType = carType.ToString(), Customer = currentUser, AppUserID = currentUser.Id, OrderDT = DateTime.Now, StartLocation = location, StartLocationID = location.Id, Deleted = false };
                newRide.Status = Status.Created;
            }
            else
            {
                newRide = new Ride() { CarType = carType.ToString(),Dispatcher = currentUser,DispatcherID = currentUser.Id, TaxiDriver = model.Driver, TaxiDriverID = model.Driver.Id, OrderDT = DateTime.Now, StartLocation = location, StartLocationID = location.Id, Deleted = false };
                newRide.Status = Status.Formed;
            }

            if (newRide != null)
            {
                try
                {
                    unitOfWork.Rides.Add(newRide);
                }
                catch (Exception e)
                {
                    return BadRequest("Error while trying to add ride to database");
                }
            }

            unitOfWork.Complete();

            return Ok();    

        }

        [HttpDelete]
        [Authorize(Roles = "AppUser")]
        [Route("CancelUserRide")]
        public IHttpActionResult CancelUserRide(int id)
        {
            Ride ride = unitOfWork.Rides.Get(id);
            if(ride == null || ride.Deleted == true)
            {
                return NotFound();
            }
            return Ok();

        }


    }
}