using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.Enums;

namespace ServicesApp.Application.Interfaces
{
    public interface IAccountService
    {
        //POST
        public Task<string?> getExitedImageName(ChangeUserImageDto changeUserImageDto);
        public Task<string?> changeImage(string image, ChangeUserImageDto changeUserImageDto);
        public Task<string?> changeFullName(ChangeFullNameDto changeFullNameDto);
        public Task<string?> changeAddress(ChangeAddressDto changeAddressDto);
        public Task<string?> changeBio(ChangeBioDto changeBioDto);
        public Task<bool> changePassword(ChangePasswordDto changePasswordDto);
        public Task<bool> forgetPassword(ForgetPasswordDto forgetPasswordDto);
        public Task<bool> resetPassword(ResetPasswordDto resetPasswordDto);
        public Task<bool> changePhone(ChangePhoneDto changePhoneDto);

        //Get
        public Task<GetAppUserDto> getUserById(string id);
        public Task<GetUserInfoForOrderDto> getUserInfoForOrderById(string id);

        public Task<bool> provideIsExists(string id);
        public Task<bool> clientIsExists(string id);

        public List<GetAppUserDto> getTopRatedProviders();
        public Task<PagedResultDto<GetAppUserDto>> getAllProviders(PagedInputsDto pagedInputsDto);

        public List<GetAppUserDto> getTopRatedClients();

        public Task<PagedResultDto<GetAppUserDto>> getAllClients(PagedInputsDto pagedInputsDto);

        public Task<PagedResultDto<GetAppUserDto>> getPagedUsers(UserType type, PagedInputsDto pagedInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getProviderReviews(PagedInputsDto pagedInputsDto);
        public Task<PagedResultDto<GetAppUserDto>> getClientReviews(PagedInputsDto pagedInputsDto);
        public Task<List<GetAppUserDto>> getProviderByServiceOrName(string searchKey);
    }
}
