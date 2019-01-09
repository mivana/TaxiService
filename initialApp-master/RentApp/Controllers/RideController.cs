﻿using RentApp.Models;
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

        // GET: api/Ride
        public IEnumerable<Ride> GetRides()
        {
            return unitOfWork.Rides.GetAll().Where(u => u.Deleted == false);
        }

        [HttpGet]
        [Authorize(Roles ="Driver,Admin")]
        [Route("GetFreeRides")]
        [ResponseType(typeof(List<Ride>))]
        public IHttpActionResult GetFreeRides()
        {
            var result = unitOfWork.Rides.Find(r => r.Status == Status.Created && !r.Deleted).ToList();
            if (result.Count() == 0)
            {
                return Ok();
                //return BadRequest("No free rides");
            }
            return Ok(result);

        }

        [HttpPut]
        [Authorize(Roles = "Admin,Driver")]
        [Route("TakeRide/{idRide}")]
        [ResponseType(typeof(Ride))]
        public IHttpActionResult TakeRide(int idRide,Ride takeRide)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            Ride ride = unitOfWork.Rides.FirstOrDefault(a => a.Id == idRide && a.Deleted == false);
            if (ride == null)
                return BadRequest();

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();
            if (!user.DriverFree)
                return BadRequest("Driver not free");
            try
            {
                user.DriverFree = false;

                unitOfWork.AppUsers.Update(user);

                ride.TaxiDriver = user;
                ride.TaxiDriverID = user.Id;
                ride.Status = Status.Accepted;

                unitOfWork.Rides.Update(ride);

                unitOfWork.Complete();

                return Ok(ride);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin,AppUser")]
        [Route("{id}")]
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
                newRide = new Ride()
                {
                    CarType = carType.ToString(),
                    Status = Status.Created,
                    Customer = currentUser,
                    AppUserID = currentUser.Id,
                    OrderDT = DateTime.Now,
                    StartLocation = location,
                    StartLocationID = location.Id,
                    Deleted = false
                };
            }
            else
            {
                newRide = new Ride()
                {
                    CarType = carType.ToString(),
                    Status = Status.Formed,
                    Dispatcher = currentUser,
                    DispatcherID = currentUser.Id,
                    //TaxiDriver = model.Driver,
                    TaxiDriverID = model.Driver.Id,
                    OrderDT = DateTime.Now,
                    StartLocation = location,
                    StartLocationID = location.Id,
                    Deleted = false
                };
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
            comment.Username = user.Username;
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
        public IHttpActionResult CancelRideComplete(CommentRideBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(model.Ride.Dispatcher != null)
            {
                return BadRequest("You can not cancel a drive made by dispatcher");
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            model.Ride.Status = Status.Cancelled;
            model.Ride.Deleted = true;

            try
            {
                unitOfWork.Rides.Update(model.Ride);
            }
            catch(Exception e)
            {
                return BadRequest("Error while trying to cancel ride");
            }

            model.UserComment.AppUser = user;
            model.UserComment.Username = user.Username;
            model.UserComment.AppUserID = user.Id;
            model.UserComment.Ride = model.Ride;
            model.UserComment.RideID = model.Ride.Id;
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

        [HttpPost]
        [Authorize(Roles = "Driver,Admin")]
        [Route("FinishRide")]
        public IHttpActionResult FinishRide(FinishRideBindingModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();
            if (user.Id == model.FinishRide.AppUserID)
                return BadRequest();

            Ride ride = unitOfWork.Rides.FirstOrDefault(r => r.Id == model.FinishRide.Id);
            if (ride == null)
                return BadRequest("No ride found");

            //If ride was successfull
            if (model.IsGood)
            {
                if (model.DStreetName == "" || model.DNumber.ToString() == "" || model.DTown == "" || model.DAreaCode.ToString() == "" || model.Price.ToString() == "")
                {
                    return BadRequest();
                }

                Address destination = new Address()
                {
                    StreetName = model.DStreetName,
                    Number = model.DNumber,
                    Town = model.DTown,
                    AreaCode = model.DAreaCode,
                    Deleted = false
                };

                try
                {
                    unitOfWork.Addresses.Add(destination);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                Location desLocation = new Location()
                {
                    XPos = 0,
                    YPos = 0,
                    Address = destination,
                    AddressID = destination.Id,
                    Deleted = false
                };

                try
                {
                    unitOfWork.Locations.Add(desLocation);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                ride.DestinationLocation = desLocation;
                ride.DestinationLocationID = desLocation.Id;
                ride.Status = Status.Successfull;

                try
                {
                    unitOfWork.Rides.Update(ride);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                user.DriverFree = true;
                user.DriverLocation = desLocation;
                user.DriverLocationId = desLocation.Id;

                try
                {
                    unitOfWork.AppUsers.Update(user);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                unitOfWork.Complete();

            }
            else //if ride failed
            {
                if (model.Content == "")
                    return BadRequest();

                ride.Status = Status.Failed;

                try
                {
                    unitOfWork.Rides.Update(ride);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                user.DriverFree = true;

                try
                {
                    unitOfWork.AppUsers.Update(user);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                Comment driverComment = new Comment()
                {
                    Content = model.Content,
                    Username = user.Username,
                    AppUser = user,
                    AppUserID = user.Id,
                    Ride = ride,
                    RideID = ride.Id,
                    DateCreated = DateTime.Now,
                    Rating = 0,
                    Deleted = false
                };

                try
                {
                    unitOfWork.Comments.Add(driverComment);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                unitOfWork.Complete();

                ///OVde negde duplira podatke u bazi, u ovom trenutku, u to sam sigurna, RESI TO!

                //unitOfWork.Complete();
            }

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "AppUser")]
        [Route("CommentRideComplete")]
        public IHttpActionResult CommentRideComplete(CommentRideBindingModel model)
        {
            //testiraj ovo da li radi, nesto mi duplira podatke!!! VAZNOOOOOOOOOOOOOOOOOOOOOOOOO
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            Ride ride = unitOfWork.Rides.FirstOrDefault(r => r.Id == model.Ride.Id);
            if (ride == null)
                return BadRequest("No ride found");

            Comment comment = new Comment()
            {
                Content = model.UserComment.Content,
                Username = user.Username,
                Rating = model.UserComment.Rating,
                AppUser = user,
                AppUserID = user.Id,
                Ride = ride,
                RideID = ride.Id,
                DateCreated = DateTime.Now,
                Deleted = false
            };

            try
            {
                unitOfWork.Comments.Add(comment);
            }
            catch (Exception e)
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