using Microsoft.AspNet.Identity.Owin;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [AllowAnonymous]
        [Route("Unique")]
        public bool Unique(string username)
        {
            var result = unitOfWork.AppUsers.Find(u => u.Username.Equals(username));
            if(result != null)
            {
                return false;
            }
            return true;
        }
       

    }
}