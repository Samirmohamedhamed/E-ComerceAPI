using System.ComponentModel.DataAnnotations;

namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class ResetPasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public int OTP { get; set; }
    }
}
