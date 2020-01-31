using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Entities.User;
using WebFramework.Api;
using Microsoft.AspNetCore.Identity;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    //[Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class UsersManagerController : BaseController
    {
        private readonly ILogger<UsersManagerController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersManagerController(ILogger<UsersManagerController> logger,
            UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ActivateUserEmailStat(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChangeUserLockoutMode(int userId, bool activate)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.LockoutEnabled = activate;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChangeUserRoles(int userId, int[] roleIds)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var roleList = new string[] { };

            foreach (var roleId in roleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());

                roleList.Append(role.Name);
            }

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.AddToRolesAsync(user, roleList);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChangeUserStat(int userId, bool activate)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.IsActive = activate;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> ChangeUserTwoFactorAuthenticationStat(int userId, bool activate)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.TwoFactorEnabled = activate;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }

        [HttpPost("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> EndUserLockout(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.LockoutEnd = null;

            var result = await _userManager.UpdateSecurityStampAsync(user);
            var updateUser = await _userManager.UpdateAsync(user);

            if (!result.Succeeded || !updateUser.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}