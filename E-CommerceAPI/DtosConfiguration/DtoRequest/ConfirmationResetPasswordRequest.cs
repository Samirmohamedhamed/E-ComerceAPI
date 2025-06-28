namespace E_CommerceAPI.DtosConfiguration.DtoRequest
{
    public class ConfirmationResetPasswordRequest
    {
        public string UserNameOrEmail { get; set; }
        public int OTP { get; set; }
    }
}
