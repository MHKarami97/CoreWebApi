using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using MyApi.Models;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        private readonly ILogger<RolesController> _logger;
        private readonly RoleManager<Role> _roleManager;

        public RolesController(ILogger<RolesController> logger, RoleManager<Role> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        [HttpGet("[action]")]
        public virtual async Task<ApiResult<List<Role>>> Get(CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

            return roles;
        }

        [HttpPost]
        public async Task<ApiResult<Role>> Get(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
                return NotFound();

            return role;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> AddOrUpdate(RoleDto role)
        {
            var result = await _roleManager.FindByNameAsync(role.Name);

            if (result != null)
                return BadRequest("این نقش موجود است");

            var roleResult = await _roleManager.CreateAsync(new Role
            {
                Name = role.Name,
                Description = role.Description
            });

            if (roleResult.Succeeded)
                return Ok();

            return BadRequest("این نقش موجود است");
        }
    }
}