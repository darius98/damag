using Damag.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(Damag.Startup))]
namespace Damag
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            CreateAdminUserAndApplicationRoles();
        }

        private void CreateAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            context.Configuration.LazyLoadingEnabled = true;

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Administrator"
                });

                var user = new ApplicationUser
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };
                var adminCreated = UserManager.Create(user, "Administrator1!");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Administrator");
                }
            }
            if (!roleManager.RoleExists("Collaborator"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "Collaborator"
                });
            }
            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new IdentityRole
                {
                    Name = "User"
                });
            }
        }
    }
}
