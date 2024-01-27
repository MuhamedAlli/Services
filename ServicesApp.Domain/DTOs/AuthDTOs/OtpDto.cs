namespace ServicesApp.Domain.DTOs.AuthDTOs
{
    public class OtpDto
    {
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(6)]
        public string? OtpCode { get; set; }
    }
}
