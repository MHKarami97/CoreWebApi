using System;
using System.Linq;
using Common;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SiteSettings _settings;

        public UserDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<SiteSettings>  settings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _settings = settings.Value;
        }

        public void InitializeData()
        {
            var setting = _settings.Identity;

            if (_userManager.Users.AsNoTracking().Any(p => p.UserName == setting.Username)) return;

            var user = new User
            {
                Birthday = DateTime.Now,
                FullName = setting.FullName,
                Gender = GenderType.Male,
                UserName = setting.Username,
                Email = setting.Email,
                PhoneNumber = setting.Phone
            };

            _userManager.CreateAsync(user, setting.Password).GetAwaiter().GetResult();

            var roles = setting.Roles.Split(',');

            foreach (var role in roles)
            {
                _roleManager.CreateAsync(new Role
                {
                    Name = role,
                    Description = "Role " + role
                }).GetAwaiter().GetResult();
            }

            //_userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
        }
    }
}