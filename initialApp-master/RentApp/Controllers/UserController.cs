using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
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

        public IEnumerable<AppUser> GetUsers()
        {
            return unitOfWork.AppUsers.GetAll().Where(u => u.Deleted == false);
        }

        [HttpGet]
        [Authorize(Roles = "AppUser,Driver,Admin")]
        [Route("GetUserById/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetUserById(int id)
        {
            if (id.ToString() == "")
                return BadRequest();

            var user = unitOfWork.AppUsers.FirstOrDefault(a => a.Id == id);
            return Ok(user);
        }

        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Route("GetAddress/{idAddress}")]
        [ResponseType(typeof(Address))]
        public IHttpActionResult GetAddress(int idAddress)
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            Location result = unitOfWork.Locations.FirstOrDefault(a => a.Id == idAddress && a.Deleted == false && a.Id == user.DriverLocationId);
            if (result == null)
                return BadRequest("No location found");
            if(result.Address == null && result.AddressID.ToString() == null)
                return BadRequest("No address found");
            if (result.Address == null && result.AddressID.ToString() != null)
            {
               var a = unitOfWork.Addresses.FirstOrDefault(aa => aa.Id == result.AddressID && aa.Deleted == false);
            }
            return Ok(result.Address);
        }

        [HttpGet]
        [Authorize(Roles = "AppUser")]
        [Route("GetUserRides")]
        [ResponseType(typeof(IEnumerable<Ride>))]
        public IHttpActionResult GetUserRides()
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if(user != null)
            {
                return Ok(user.CustomerRides);
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = "AppUser,Driver,Admin")]
        [Route("GetActiveInfo")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetActiveInfo()
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);

            if (user != null)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(user);
            }
            return BadRequest();

        }

        [HttpGet]
        [Authorize(Roles = "AppUser,Driver,Admin")]
        [Route("GetUserRole")]
        [ResponseType(typeof(AppUser.UserRole))]
        public IHttpActionResult GetUserRole()
        {
            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);

            if (user != null)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(user.Role);
            }
            return BadRequest();

        }

        [HttpGet]
        [Authorize(Roles = "AppUser,Driver,Admin")]
        [Route("GetFreeDrivers")]
        [ResponseType(typeof(List<AppUser>))]
        public IHttpActionResult GetFreeDrivers()
        {
            List<AppUser> freeDrivers = unitOfWork.AppUsers.Find(u => u.Role == AppUser.UserRole.Driver && u.DriverFree == true).ToList();
            if(freeDrivers.Count() != 0)
            {
                return Ok(freeDrivers);
            }
            return BadRequest("No free drivers at this moment");

        }


        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            if (model.Role != "AppUser" && model.Role != "Driver")
            {
                return BadRequest();
            }

            AppUser.UserRole userRole;

            if (model.Role == "AppUser")
            {
                userRole = AppUser.UserRole.AppUser;
            }
            else
            {
                userRole = AppUser.UserRole.Driver;
            }

            var result = unitOfWork.AppUsers.Find(u => u.Username == model.Username);
            if (result.Count() != 0)
                return BadRequest("Username not unique");

            var resultE = unitOfWork.AppUsers.Find(u => u.Email == model.Email);
            if (resultE.Count() != 0)
                return BadRequest("Email has account");

            var appUser = new AppUser()
            {
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                Username = model.Username,
                Role = userRole,
                JMBG = model.JMBG,
                ContactNumber = model.ContactNumber,
                Blocked = false,
                DriverFree = true,
                Deleted = false
            };

            var user = new RAIdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
                AppUser = appUser,
                PasswordHash = RAIdentityUser.HashPassword(model.Password)
            };

            UserManager.Create(user);
            UserManager.AddToRole(user.Id, model.Role);

            return Ok();
        }



        [HttpPut]
        [Authorize(Roles ="Admin,AppUser,Driver")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult PutUser(int id, AppUser user)
        {
            //OVO SE ZAJEBALO NESTO IDK ZASTO... fuck
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                var result = unitOfWork.AppUsers.Find(n => n.Username == user.Username && n.Id.ToString() != user.Id.ToString());
                if (result.Count() != 0)
                        return BadRequest("Username not Unique");

                var CurUser = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
                if (CurUser == null)
                {
                    return BadRequest();
                }
                CurUser.FullName = user.FullName;
                CurUser.JMBG = user.JMBG;
                CurUser.Gender = user.Gender;
                CurUser.ContactNumber = user.ContactNumber;

                    unitOfWork.AppUsers.Update(CurUser);
                    unitOfWork.Complete();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpPut]
        [Route("UpdateUsername")]
        [Authorize(Roles = "Admin,AppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult UpdateUsername(int id, AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var us = unitOfWork.AppUsers.FirstOrDefault(i => i.Id == id);
            if(us == null)
            {
                return BadRequest();
            }

            //if (id != user.Id)
            //{
            //    return BadRequest();
            //}

            try
            {
                var result = unitOfWork.AppUsers.Find(n => n.Username == user.Username);
                if (result.Count() != 0)
                    return BadRequest("Username not Unique");

                user.Username = user.Username;

                unitOfWork.AppUsers.Update(user);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("RegisterDriver")]
        public IHttpActionResult RegisterDriver(RegisterDriverBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            if (model.Role != "AppUser" && model.Role != "Driver")
            {
                return BadRequest();
            }

            AppUser.UserRole userRole;

            if (model.Role == "AppUser")
            {
                userRole = AppUser.UserRole.AppUser;
            }
            else
            {
                userRole = AppUser.UserRole.Driver;
            }

            var result = unitOfWork.AppUsers.Find(u => u.Username == model.Username);
            if (result.Count() != 0)
                return BadRequest("Username not unique");

            var resultE = unitOfWork.AppUsers.Find(u => u.Email == model.Email);
            if (resultE.Count() != 0)
                return BadRequest("Email has account");

            var appUser = new AppUser()
            {
                Email = model.Email,
                FullName = model.FullName,
                Gender = model.Gender,
                Username = model.Username,
                Role = userRole,
                JMBG = model.JMBG,
                ContactNumber = model.ContactNumber,
                Blocked = false,
                DriverFree = true,
                Deleted = false
            };

            var user = new RAIdentityUser()
            {
                UserName = model.Email,
                Email = model.Email,
                AppUser = appUser,
                PasswordHash = RAIdentityUser.HashPassword(model.Password)
            };

            UserManager.Create(user);
            UserManager.AddToRole(user.Id, model.Role);

            CarType ct;
            if(model.CarType == "Standard")
            {
                ct = CarType.Standard;
            }
            else
            {
                ct = CarType.Combi;
            }

            var resultT = unitOfWork.Cars.Find(u => u.TaxiNumber == model.TaxiNumber);
            if (resultT.Count() != 0)
                return BadRequest("TaxiNumber not unique");

            Car newCar = new Car() { RegistrationPlate = model.RegistrationPlate, YearMade = model.YearMade, TaxiNumber = model.TaxiNumber, CarType = ct };
            newCar.Driver = user.AppUser;
            newCar.AppUserID = user.AppUser.Id;

            unitOfWork.Cars.Add(newCar);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Driver")]
        [Route("ChangeDriverLocation")]
        public IHttpActionResult ChangeDriverLocation(Address newAddress)
        {
            if (newAddress == null)
                return BadRequest();

            var user = unitOfWork.AppUsers.FirstOrDefault(u => u.Email == User.Identity.Name && u.Deleted == false);
            if (user == null)
                return BadRequest();

            if(user.DriverLocation == null && user.DriverLocationId == null)
            {
                Address address = new Address()
                {
                    StreetName = newAddress.StreetName,
                    Number = newAddress.Number,
                    Town = newAddress.Town,
                    AreaCode = newAddress.AreaCode,
                    Deleted = false
                };

                try
                {
                    unitOfWork.Addresses.Add(address);
                }
                catch(Exception e)
                {
                    return BadRequest();
                }

                Location location = new Location()
                {
                    XPos = 0,
                    YPos = 0,
                    Address = address,
                    AddressID = address.Id,
                    Deleted = false
                };

                try
                {
                    unitOfWork.Locations.Add(location);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                user.DriverLocation = location;

                try
                {
                    unitOfWork.AppUsers.Update(user);
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

            }
            else
            {
                Location location = unitOfWork.Locations.FirstOrDefault(l => l.Id == user.DriverLocationId);
                Address address = unitOfWork.Addresses.FirstOrDefault(a => a.Id == location.AddressID);

                address.StreetName = newAddress.StreetName;
                address.Number = newAddress.Number;
                address.Town = newAddress.Town;
                address.AreaCode = newAddress.AreaCode;

                try
                {
                    unitOfWork.Addresses.Update(address);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                location.Address = address;
                location.AddressID = address.Id;

                try
                {
                    unitOfWork.Locations.Update(location);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }

                user.DriverLocation = location;

                try
                {
                    unitOfWork.AppUsers.Update(user);
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

            }

            return Ok();
        }



        private bool UserExists(int id)
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