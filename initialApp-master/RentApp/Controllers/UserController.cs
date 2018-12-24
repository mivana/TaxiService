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

    }
}