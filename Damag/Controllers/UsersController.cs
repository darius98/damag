using Damag.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Damag.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db = ApplicationDbContext.Create();

        public ActionResult Index()
        {
            var users = (from user in db.Users orderby user.UserName select user).ToList();
            var roles = (from role in db.Roles select role).ToList();
            ViewBag.UsersList = users;
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var role = (from r in user.Roles select r).FirstOrDefault();
                if (role == null)
                {
                    userRoles[user.Id] = roles.Where(r => r.Name == "User").First().Name;
                } else
                {
                    userRoles[user.Id] = roles.Where(r => r.Id == role.RoleId).First().Name;
                }
            }
            ViewBag.UserRoles = userRoles;
            return View();
        }

        public ActionResult Edit(string id) {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            if (userRole == null)
            {
                ViewBag.userRole = (from role in db.Roles where role.Name == "User" select role.Id).First();
            } else
            {
                ViewBag.userRole = userRole.RoleId;
            }
            return View(user);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();
            var roles = from role in db.Roles select role;
            foreach (var role in roles) 
            { 
                selectList.Add(new SelectListItem 
                { 
                    Value = role.Id.ToString(), 
                    Text = role.Name.ToString() 
                }); 
            }
            return selectList;
        }


        [HttpPut]
        public ActionResult Edit(string id, ApplicationUser newData)
        {
            ApplicationUser user = db.Users.Find(id);
            user.AllRoles = GetAllRoles();
            var userRole = user.Roles.FirstOrDefault();
            if (userRole == null)
            {
                ViewBag.userRole = (from role in db.Roles where role.Name == "User" select role.Id).First();
            }
            else
            {
                ViewBag.userRole = userRole.RoleId;
            }
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                if (TryUpdateModel(user))
                {
                    user.UserName = newData.UserName;
                    user.Email = newData.Email;
                    var roles = from role in db.Roles select role;
                    foreach (var role in roles) 
                    { 
                        UserManager.RemoveFromRole(id, role.Name); 
                    }
                    var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
                    UserManager.AddToRole(id, selectedRole.Name);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            } catch (Exception e)
            {
                Response.Write(e.Message);
                return View(user);
            }
        }
    }
}