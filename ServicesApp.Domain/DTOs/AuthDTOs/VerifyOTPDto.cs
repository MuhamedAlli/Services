namespace ServicesApp.Domain.DTOs.AuthDTOs
{
    public class VerifyOTPDto
    {
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;

        [StringLength(maximumLength: 6, ErrorMessage = "OTP Code is not valid!", MinimumLength = 6)]
        public string OtpCode { get; set; } = null!;
    }
}
