using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class ResendEmailConfirmationRequest
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;

    }
}
