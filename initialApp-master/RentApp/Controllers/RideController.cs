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

        // GET: api/Ride
        public IEnumerable<Ride> GetRides()
        {
            return unitOfWork.Rides.GetAll().Where(u => u.Deleted == false);
        }

        [HttpGet]
        [Authorize(Roles = "Driver,Admin")]
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

        [HttpPost]
        [Authorize(Roles = "Driver,Admin,AppUser")]
        [Route("Search")]
        [ResponseType(typeof(List<Ride>))]
        public IHttpActionResult Search(SearchBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            List<Ride> results = new List<Ride>();

            switch (user.Role)
            {
                case AppUser.UserRole.AppUser:

                    results = user.CustomerRides.Where(r => model.DateFrom == null || r.OrderDT.Date.CompareTo((DateTime)model.DateFrom.Value.Date) >= 0)
                              .Where(r => model.DateTo == null || r.OrderDT.Date.CompareTo((DateTime)model.DateTo.Value.Date) <= 0)
                              .Where(r => model.PriceFrom == "" || r.Price >= double.Parse(model.PriceFrom))
                              .Where(r => model.PriceTo == "" || r.Price <= double.Parse(model.PriceTo))
                              .Where(r => model.RatingFrom == "" || r.UserComment.First().Rating >= Int32.Parse(model.RatingFrom))
                              .Where(r => model.RatingTo == "" || r.UserComment.First().Rating <= Int32.Parse(model.RatingTo)).ToList();

                    break;

                case AppUser.UserRole.Driver:
                    if (model.SearchFree == true)
                    {
                        List<Ride> rides = unitOfWork.Rides.Find(r => r.Status == Status.Created).ToList();
                        results = rides.Where(r => model.DateFrom == null || r.OrderDT.Date.CompareTo((DateTime)model.DateFrom.Value.Date) >= 0)
                              .Where(r => model.DateTo == null || r.OrderDT.Date.CompareTo((DateTime)model.DateTo.Value.Date) <= 0)
                              .Where(r => model.PriceFrom == "" || r.Price >= double.Parse(model.PriceFrom))
                              .Where(r => model.PriceTo == "" || r.Price <= double.Parse(model.PriceTo))
                              .Where(r => model.RatingFrom == "" || r.UserComment.First().Rating >= Int32.Parse(model.RatingFrom))
                              .Where(r => model.RatingTo == "" || r.UserComment.First().Rating <= Int32.Parse(model.RatingTo)).ToList();
                    }

                    if (model.SearchMineD == true)
                    {
                        results = user.DriverRides.Where(r => model.DateFrom == null || r.OrderDT.Date.CompareTo((DateTime)model.DateFrom.Value.Date) >= 0)
                              .Where(r => model.DateTo == null || r.OrderDT.Date.CompareTo((DateTime)model.DateTo.Value.Date) <= 0)
                              .Where(r => model.PriceFrom == "" || r.Price >= double.Parse(model.PriceFrom))
                              .Where(r => model.PriceTo == "" || r.Price <= double.Parse(model.PriceTo))
                              .Where(r => model.RatingFrom == "" || r.UserComment.First().Rating >= Int32.Parse(model.RatingFrom))
                              .Where(r => model.RatingTo == "" || r.UserComment.First().Rating <= Int32.Parse(model.RatingTo)).ToList();
                    }
                    break;

                case AppUser.UserRole.Admin:
                    if (model.SearchAll == true)
                    {
                        List<Ride> rides = unitOfWork.Rides.GetAll().ToList();
                        results = rides.Where(r => model.DateFrom == null || r.OrderDT.Date.CompareTo((DateTime)model.DateFrom.Value.Date) >= 0)
                              .Where(r => model.DateTo == null || r.OrderDT.Date.CompareTo((DateTime)model.DateTo.Value.Date) <= 0)
                              .Where(r => model.PriceFrom == "" || r.Price >= double.Parse(model.PriceFrom))
                              .Where(r => model.PriceTo == "" || r.Price <= double.Parse(model.PriceTo))
                              .Where(r => model.RatingFrom == "" || r.UserComment.First().Rating >= Int32.Parse(model.RatingFrom))
                              .Where(r => model.RatingTo == "" || r.UserComment.First().Rating <= Int32.Parse(model.RatingTo)).ToList();
                    }

                    if (model.SearchMine == true)
                    {
                        results = user.DispatcherRides.Where(r => model.DateFrom == null || r.OrderDT.Date.CompareTo((DateTime)model.DateFrom.Value.Date) >= 0)
                              .Where(r => model.DateTo == null || r.OrderDT.Date.CompareTo((DateTime)model.DateTo.Value.Date) <= 0)
                              .Where(r => model.PriceFrom == "" || r.Price >= double.Parse(model.PriceFrom))
                              .Where(r => model.PriceTo == "" || r.Price <= double.Parse(model.PriceTo))
                              .Where(r => model.RatingFrom == "" || r.UserComment.First().Rating >= Int32.Parse(model.RatingFrom))
                              .Where(r => model.RatingTo == "" || r.UserComment.First().Rating <= Int32.Parse(model.RatingTo)).ToList();
                    }
                    break;

            }

            return Ok(results);

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("SearchAdmin")]
        [ResponseType(typeof(List<Ride>))]
        public IHttpActionResult SearchAdmin(SearchAdminBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            List<Ride> results = new List<Ride>();

            List<Ride> rides = unitOfWork.Rides.GetAll().ToList();
            results = rides.Where(r => model.DName == "" || r.TaxiDriver != null && r.TaxiDriver.FullName.Contains(model.DName))
                            .Where(r => model.DSurname == "" || r.TaxiDriver != null && r.TaxiDriver.FullName.Contains(model.DSurname))
                            .Where(r => model.CName == "" || r.Customer != null && r.Customer.FullName.Contains(model.CName))
                            .Where(r => model.CSurname == "" || r.Customer != null && r.Customer.FullName.Contains(model.CSurname)).ToList();


            return Ok(results);
        }

        [HttpPost]
        [Authorize(Roles = "Driver,Admin,AppUser")]
        [Route("Sort")]
        [ResponseType(typeof(List<Ride>))]
        public IHttpActionResult Sort(SortBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            List<Ride> results = new List<Ride>();
            List<Ride> temp = new List<Ride>();

            if (model.SortBy == "Date")
            {
                results = model.ResultList.OrderBy(r => r.OrderDT).ToList();
            }
            if(model.SortBy == "Rating")
            {
                List<Ride> noRatingRides = new List<Ride>();
                List<Ride> yesRatingRides = new List<Ride>();
                foreach (Ride r in model.ResultList)
                {
                    if(r.UserComment.Count() == 0)
                    {
                        noRatingRides.Add(r);
                    }
                    else
                    {
                        yesRatingRides.Add(r);
                    }
                }
                
                temp = yesRatingRides.OrderBy(r => r.UserComment.First().Rating).ToList();
                if (noRatingRides.Count() != 0)
                {
                    results = noRatingRides;
                    results.AddRange(temp);
                }
                else
                {
                    results = temp;
                }


            }

            return Ok(results);

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

            AppUser driver = new AppUser();
            if (currentUser.Role == AppUser.UserRole.Admin)
            {
                driver = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == model.Driver.Email && u.Deleted == false);
                driver.DriverFree = false;

                try
                {
                    unitOfWork.AppUsers.Update(driver);
                }
                catch (Exception e)
                {
                    return BadRequest("Error while trying to add ride to database");
                }
            }
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
                    TaxiDriverID = driver.Id,
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

            //OVO SAM DODALA POSLEDNJE< AKO PUCA TO JE ZBOG OVOGA! DA ZNAS
            if (currentUser.Role == AppUser.UserRole.Admin)
            {
                driver.DriverLocation = location;
                driver.DriverLocationId = location.AddressID;

                try
                {
                    unitOfWork.AppUsers.Update(driver);
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("AssignDriver")]
        public IHttpActionResult AssignDriver(AssignDriverBindingModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            var userDriver = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == model.Driver.Email && u.Deleted == false);
            if (userDriver == null)
                return BadRequest();

            Ride ride = unitOfWork.Rides.FirstOrDefault(r => r.Id == model.Ride.Id);
            if (ride == null)
                return BadRequest("No ride found");


            userDriver.DriverFree = false;
            userDriver.DriverLocation = ride.StartLocation;

            try
            {
                unitOfWork.AppUsers.Update(userDriver);
            }
            catch(Exception e)
            {
                return BadRequest();
            }

            ride.Status = Status.Proccessed;
            ride.Dispatcher = user;
            ride.DispatcherID = user.Id;
            ride.TaxiDriver = userDriver;
            ride.TaxiDriverID = userDriver.Id;

            try
            {
                unitOfWork.Rides.Update(ride);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.Complete();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

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