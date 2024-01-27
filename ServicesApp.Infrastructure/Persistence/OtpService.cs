using AutoMapper;
using ServicesApp.Domain.DTOs.AuthDTOs;

namespace ServicesApp.Infrastructure.Persistence
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public OtpService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public string GenerateOtp()
        {
            // Create an instance of the Random class
            Random random = new Random();

            // Generate a random 6-digit OTP
            int otp = random.Next(100000, 999999);

            return otp.ToString();
        }

        public void StoreOtp(OtpDto otpDto)
        {
            if (!string.IsNullOrEmpty(otpDto.OtpCode) && !string.IsNullOrEmpty(otpDto.PhoneNumber))
            {
                var model = _mapper.Map<Otp>(otpDto);
                model.ExpireDate = DateTime.Now.AddMinutes(5);
                _dbContext.Otps.Add(model);
                _dbContext.SaveChanges();
            }
        }

        public bool ValidateOtp(VerifyOTPDto verifyOTPDto)
        {
            var otpEntity = _dbContext.Otps
           .Where(o => o.PhoneNumber == verifyOTPDto.PhoneNumber && o.ExpireDate > DateTime.Now)
           .OrderByDescending(o => o.ExpireDate)
           .FirstOrDefault();

            if (otpEntity != null)
            {
                // Validate entered OTP against stored hash
                if (otpEntity.OtpCode!.Equals(verifyOTPDto.OtpCode))
                {
                    var user = _dbContext.Users.FirstOrDefault(u => u.UserName == verifyOTPDto.PhoneNumber);
                    if (user is not null)
                    {
                        user.IsVerified = true;
                        _dbContext.SaveChanges();
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
