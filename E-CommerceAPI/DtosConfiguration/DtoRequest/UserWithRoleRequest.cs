using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class UserWithRoleRequest
    {
        public string UserId { get; set; }
        public string SelectedRole { get; set; }
    }
}
