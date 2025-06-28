namespace E_CommerceAPI.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public int OTP { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }
    }
}
