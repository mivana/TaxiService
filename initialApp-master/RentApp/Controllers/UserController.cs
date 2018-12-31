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
        [Authorize(Roles ="Admin,AppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult PutUser(int id, AppUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                var result = unitOfWork.AppUsers.Find(n => n.Username == user.Username && n.Id.ToString() != user.Id.ToString());
                if (result.Count() != 0)
                        return BadRequest("Username not Unique");

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