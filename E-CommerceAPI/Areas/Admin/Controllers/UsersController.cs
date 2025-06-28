using E_CommerceAPI.DtosConfiguration.DtoRequest;
using E_CommerceAPI.DtosConfiguration.DtoResponse;
using EComerceTestSamir.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IApplicationUserRepository applicationUserRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRoleRepository roleRepository;
        private readonly IEmailSender emailSender;

        public UsersController(IApplicationUserRepository applicationUserRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository, IEmailSender emailSender)
        {
            this.applicationUserRepository = applicationUserRepository;
            this.userManager = userManager;
            this.roleRepository = roleRepository;
            this.emailSender = emailSender;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = applicationUserRepository.Get();
            return Ok(users.Adapt<IEnumerable<UserResponse>>());
        }
       
        [HttpPost("{id}")]
        public async Task<IActionResult> GetOne([FromRoute] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is not null)
            {
                return Ok(user.Adapt<UserResponse>());
            }
            return BadRequest();
        }
        [HttpPost("ChangeRole")]
        public async Task<IActionResult> ChangeRole(UserWithRoleRequest userWithRoleRequest)
        {
            if (userWithRoleRequest.SelectedRole is null)
            {
                return BadRequest("The new role is not specified.");
            }

            var user = await userManager.FindByIdAsync(userWithRoleRequest.UserId);
            if (user != null)
            {
                var currentRoles = await userManager.GetRolesAsync(user);

                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return BadRequest("Failed to remove current roles.");
                }

                var addResult = await userManager.AddToRoleAsync(user, userWithRoleRequest.SelectedRole);
                if (!addResult.Succeeded)
                {
                    return BadRequest("Failed to add the new role.");
                }

                return Ok("Role changed successfully.");
            }

            return BadRequest("User not found.");
        }
        [HttpPost("LockUnLock/{id}")]
        public async Task<IActionResult> LockUnLock([FromRoute] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                user.LockoutEnd = DateTime.UtcNow;
            }
            else 
            {
                user.LockoutEnd = DateTime.UtcNow.AddDays(1);
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Failed to update lockout status.");
            }

            return Ok("User lockout status updated successfully.");
        }
    }
}
