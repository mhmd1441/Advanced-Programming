using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = _userManager.Users.ToList();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.JobTitle,
                    Role = roles.FirstOrDefault() ?? "Employee"
                });
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(string id, UpdateUserRoleDto dto)
        {
            var allowedRoles = new[] { "Admin", "Manager", "Employee" };

            if (!allowedRoles.Contains(dto.Role))
                return BadRequest("Invalid role");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors);

            var addResult = await _userManager.AddToRoleAsync(user, dto.Role);

            if (!addResult.Succeeded)
                return BadRequest(addResult.Errors);

            return Ok("User role updated successfully");
        }
    }
}
