using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.DBIntializer
{
    public class DbIntializer : IDbIntialzer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public DbIntializer(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager , ApplicationDbContext context)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            _dbContext = context;
        }
        public void Intialize()
        {
            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Customer")).GetAwaiter().GetResult();


                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "Admin123",
                    Email = "Admin123@gmail.com",
                    EmailConfirmed = true,
                    Name = "Admin"
                }, password: "@Admin123").GetAwaiter().GetResult();
                var user = _userManager.FindByEmailAsync("Admin123@gmail.com").GetAwaiter().GetResult();
                if (user!= null)
                {
                    _userManager.AddToRoleAsync(user
                    , "Admin").GetAwaiter().GetResult();
                }

                try
                {
                    if (_dbContext.Database.GetPendingMigrations().Any())
                        _dbContext.Database.Migrate();
                }
                catch (Exception ex)
                { Console.WriteLine(ex.ToString()); }

            }
            
        }
    }
}
