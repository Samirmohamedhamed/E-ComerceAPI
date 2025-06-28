using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class ForgetPasswordRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;
    }
}
