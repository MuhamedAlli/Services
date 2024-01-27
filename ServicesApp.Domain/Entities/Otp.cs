namespace ServicesApp.Domain.Entities
{
    public class Otp
    {
        public int Id { get; set; }
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;
        [StringLength(6)]
        public string? OtpCode { get; set; }
        public DateTime ExpireDate { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
