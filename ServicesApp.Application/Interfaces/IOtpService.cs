using ServicesApp.Domain.DTOs.AuthDTOs;

namespace ServicesApp.Application.Interfaces
{
    public interface IOtpService
    {
        public string GenerateOtp();
        public void StoreOtp(OtpDto otpDto);
        public bool ValidateOtp(VerifyOTPDto verifyOTPDto);
    }
}
