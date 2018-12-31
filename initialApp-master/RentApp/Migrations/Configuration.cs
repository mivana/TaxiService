namespace RentApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using RentApp.Models.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RentApp.Persistance.RADBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RentApp.Persistance.RADBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Driver"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Driver" };

                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "AppUser"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "AppUser" };

                manager.Create(role);
            }

            context.AppUsers.AddOrUpdate(

                  u => u.FullName,

                  new AppUser() { FullName = "Admin Adminovic", Username = "admin", JMBG = "023", Email = "admin", Gender = "Male", Role = AppUser.UserRole.Admin, ContactNumber = "025888777" }

            );

            context.AppUsers.AddOrUpdate(

                p => p.FullName,

                new AppUser() { FullName = "AppUser AppUserovic" }

            );

            context.AppUsers.AddOrUpdate(

                  u => u.FullName,

                  new AppUser() { FullName = "Adminin Adminirovic", Username = "adminin", JMBG = "589", Email = "adminin", Gender = "Female", Role = AppUser.UserRole.Admin, ContactNumber = "021444777" }

            );

            context.AppUsers.AddOrUpdate(

                p => p.FullName,

                new AppUser() { FullName = "Adminic Adminic", Username = "adminic", JMBG = "021", Email = "adminic", Gender = "Male", Role = AppUser.UserRole.Admin, ContactNumber = "0218884785" }

            );

            context.AppUsers.AddOrUpdate(

                  u => u.FullName,

                  new AppUser() { FullName = "Adminov Adminovovic", Username = "adminov", JMBG = "011", Email = "adminov", Gender = "Female", Role = AppUser.UserRole.Admin, ContactNumber = "023587548" }

            );


            context.SaveChanges();

            var userStore = new UserStore<RAIdentityUser>(context);
            var userManager = new UserManager<RAIdentityUser>(userStore);

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Admin Adminovic");
                var user = new RAIdentityUser() { Id = "admin", UserName = "admin"    , Email = "admin@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("admin"), AppUserId = _appUser.Id };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "appu"))

            {

                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "AppUser AppUserovic");
                var user = new RAIdentityUser() { Id = "appu", UserName = "appu", Email = "appu@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("appu"), AppUserId = _appUser.Id };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "AppUser");

            }

            if (!context.Users.Any(u => u.UserName == "adminin"))
            {
                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Adminin Adminirovic");
                var user = new RAIdentityUser() { Id = "adminin", UserName = "adminin", Email = "adminin@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("adminin"), AppUserId = _appUser.Id };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(u => u.UserName == "adminic"))

            {

                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Adminic Adminic");
                var user = new RAIdentityUser() { Id = "adminic", UserName = "adminic", Email = "adminic@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("adminic"), AppUserId = _appUser.Id };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");

            }

            if (!context.Users.Any(u => u.UserName == "adminov"))
            {
                var _appUser = context.AppUsers.FirstOrDefault(a => a.FullName == "Adminov Adminovovic");
                var user = new RAIdentityUser() { Id = "adminov", UserName = "adminov", Email = "adminov@yahoo.com", PasswordHash = RAIdentityUser.HashPassword("adminov"), AppUserId = _appUser.Id };
                userManager.Create(user);
                userManager.AddToRole(user.Id, "Admin");
            }

        }
    }
}
