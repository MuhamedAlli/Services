using ServicesApp.Domain.DTOs.AuthDTOs;

namespace ServicesApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterStepOne(RegisterStepOneDto regiserStepOneDto);
        Task<bool> RegisterStepTwo(string imageName, RegisterStepTwoDto regiserStepTwoDto);
        Task<AuthDto> Login(LoginDto loginDto);
        //Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthDto> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
    }
}
