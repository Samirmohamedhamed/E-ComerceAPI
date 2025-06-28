using E_CommerceAPI.DtosConfiguration.DtoResponse;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
       
        private readonly IRoleRepository roleRepository;
       

        public RolesController(IRoleRepository roleRepository)
        {
          
            this.roleRepository = roleRepository;
           
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = roleRepository.Get();
            return Ok(roles.Adapt<IEnumerable<RoleResponse>>());
        }
       
    }
}
