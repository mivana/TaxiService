using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

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

        [HttpPut]
        [Authorize(Roles = "Admin,AppUser")]
        [ResponseType(typeof(Ride))]
        public IHttpActionResult PutRide(int id, Ride ride)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ride.Id)
            {
                return BadRequest();
            }

            try
            {
                Address editAddress = ride.StartLocation.Address;

                unitOfWork.Addresses.Update(editAddress);
                unitOfWork.Rides.Update(ride);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RideExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);

        }

        //[HttpPut]
        //[Authorize(Roles = "Admin,AppUser")]
        //[ResponseType(typeof(Ride))]
        //public IHttpActionResult PutEditRide(int id, Ride ride)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var us = unitOfWork.AppUsers.FirstOrDefault(i => i.Id == id);
        //    if (us == null)
        //    {
        //        return BadRequest();
        //    }


        //    try
        //    {
        //        Address editAddress = ride.StartLocation.Address;

        //        unitOfWork.Addresses.Update(editAddress);

        //        unitOfWork.Rides.Update(ride);
        //        unitOfWork.Complete();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!RideExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return Ok(ride);
        //}


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

        [HttpPost]
        [Authorize(Roles = "AppUser,Driver")]
        [Route("AddComment")]
        public IHttpActionResult AddComment(Comment comment)
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if(user == null)
            {
                return BadRequest();
            }

            comment.AppUser = user;
            comment.AppUserID = user.Id;
            comment.RideID = comment.Ride.Id;
            comment.DateCreated = DateTime.Now;

            try
            {
                unitOfWork.Comments.Add(comment);
                unitOfWork.Complete();

                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "AppUser")]
        [Route("CancelRideComplete")]
        public IHttpActionResult CancelRideComplete(CancelRideBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(model.CancelRide.Dispatcher != null)
            {
                return BadRequest("You can not cancel a drive made by dispatcher");
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            model.CancelRide.Status = Status.Cancelled;
            model.CancelRide.Deleted = true;

            try
            {
                unitOfWork.Rides.Update(model.CancelRide);
            }
            catch(Exception e)
            {
                return BadRequest("Error while trying to cancel ride");
            }

            model.UserComment.AppUser = user;
            model.UserComment.AppUserID = user.Id;
            model.UserComment.Ride = model.CancelRide;
            model.UserComment.RideID = model.CancelRide.Id;
            model.UserComment.DateCreated = DateTime.Now;
            model.UserComment.Deleted = false;

            try
            {
                unitOfWork.Comments.Add(model.UserComment);
            }
            catch(Exception e)
            {
                return BadRequest("Error while trying to add Comment");
            }

            unitOfWork.Complete();

            return Ok();

        }

        private bool RideExists(int id)
        {
            var entity = unitOfWork.AppUsers.Get(id);

            if (entity == null || entity.Deleted == true)
            {
                return false;
            }

            return true;
        }


    }
}