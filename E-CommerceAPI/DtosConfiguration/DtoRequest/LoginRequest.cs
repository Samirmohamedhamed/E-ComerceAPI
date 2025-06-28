using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class LoginRequest
    {

        [Required]
        public string UserNameOrEmail { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
