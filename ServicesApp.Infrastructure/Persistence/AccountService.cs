using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Infrastructure.Persistence
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public AccountService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<string?> changeFullName(ChangeFullNameDto changeFullNameDto)
        {
            var user = await _userManager.FindByIdAsync(changeFullNameDto.ApplicationUserId);

            if (user is null)
                return null;

            user.FullName = changeFullNameDto.FullName;
            var result = await _userManager.UpdateAsync(user);

            if(!result.Succeeded)
                return null;

            return user.FullName;
        }
        public async Task<string?> changeAddress(ChangeAddressDto changeAddressDto)
        {
            var user = await _userManager.FindByIdAsync(changeAddressDto.ApplicationUserId);

            if (user is null)
                return null;

            user.Address = changeAddressDto.Address;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return null;

            return user.Address;
        }
        public async Task<string?> changeBio(ChangeBioDto changeBioDto)
        {
            var user = await _userManager.FindByIdAsync(changeBioDto.ApplicationUserId);

            if (user is null)
                return null;

            user.Bio = changeBioDto.Bio;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return null;

            return user.Bio;
        }
        public async Task<string?> changeImage(string image, ChangeUserImageDto changeUserImageDto)
        {
            var user = await _userManager.FindByIdAsync(changeUserImageDto.ApplicationUserId);
            if (user is null)
                return null;
            user.Image = image;
           var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return null;

            return image;
        }

        public async Task<string?> getExitedImageName(ChangeUserImageDto changeUserImageDto)
        {
            var user = await _userManager.FindByIdAsync(changeUserImageDto.ApplicationUserId);
            if (user is null || user.Image is null)
                return null;

            return user.Image;
        }

        public async Task<bool> changePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(changePasswordDto.ApplicationUserId);
            if (user is null)
                return false;
            if (!changePasswordDto.NewPassword.Equals(changePasswordDto.ConfirmPassword))
                return false;

            var result = await _userManager.ChangePasswordAsync(user , changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> forgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            var user =await _userManager.Users.FirstOrDefaultAsync(
                u=>u.Id==forgetPasswordDto.ApplicationUserId&&u.PhoneNumber==forgetPasswordDto.PhoneNumber);
            if (user is null)
                return false;

            return true;
        }

        public async Task<bool> resetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDto.ApplicationUserId);

            if (user is null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user,token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
                return false;

            return true;
        }
        public async Task<bool> changePhone(ChangePhoneDto changePhoneDto)
        {
            var user = await _userManager.FindByIdAsync(changePhoneDto.ApplicationUserId);
            if (user is null)
                return false;

            user.UserName = changePhoneDto.PhoneNumber;
            user.PhoneNumber = changePhoneDto.PhoneNumber;
            user.IsVerified = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return false;

            return true;
        }

        //Get User By Id 
        public async Task<GetAppUserDto> getUserById(string id)
        {
            var user =await _userManager.FindByIdAsync(id);
            if(user is not null)
            {
                var userDto = _mapper.Map<GetAppUserDto>(user);
                return userDto;
            }

            return null;
        }

        public List<GetAppUserDto> getTopRatedProviders()
        {
            var providers = _userManager.Users.Where(u=>u.Type == UserType.Provider).OrderBy(u => u.Rate).Take(10).ToList();

            return _mapper.Map<List<GetAppUserDto>>(providers);           
        }

        public List<GetAppUserDto> getTopRatedClients()
        {
            var clients = _userManager.Users.Where(u => u.Type == UserType.Client).OrderByDescending(u => u.Rate).Take(10).ToList();

            return _mapper.Map<List<GetAppUserDto>>(clients);
           
        }

        public async Task<PagedResultDto<GetAppUserDto>> getAllProviders(PagedInputsDto pagedInputsDto)
        {
            var pagedResultDto = await getPagedUsers(UserType.Provider, pagedInputsDto);

            return pagedResultDto;
        }

        public async Task<PagedResultDto<GetAppUserDto>> getAllClients(PagedInputsDto pagedInputsDto)
        {
            var pagedResultDto = await getPagedUsers(UserType.Client, pagedInputsDto);

            return pagedResultDto;
        }
        public async Task<PagedResultDto<GetAppUserDto>> getPagedUsers(UserType type,PagedInputsDto pagedInputsDto)
        {
            // Calculate the number of users to skip based on the page and pageSize
            int usersToSkip = (pagedInputsDto.Page - 1) * pagedInputsDto.PageSize;
            // Retrieve total count of users
            int totalCount = await _userManager.Users.Where(u=>u.Type==type).CountAsync();
            int totalPages = totalCount % pagedInputsDto.PageSize == 0 ? (totalCount / pagedInputsDto.PageSize) : ((totalCount / pagedInputsDto.PageSize) + 1);

            // Retrieve users from UserManager with pagination
            var users = await _userManager.Users
                .Where(u => u.Type == type)
                .OrderBy(u => u.Id) // Order by some property, adjust as needed
                .Skip(usersToSkip)
                .Take(pagedInputsDto.PageSize)
                .ToListAsync();

            // Map users to DTOs
            var usersDto = _mapper.Map<IEnumerable<GetAppUserDto>>(users);

            var pagedResultDto = new PagedResultDto<GetAppUserDto>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = pagedInputsDto.Page,
                PageSize = pagedInputsDto.PageSize,
                Data = usersDto
            };

            return pagedResultDto;
        }

        public Task<PagedResultDto<GetAppUserDto>> getProviderReviews(PagedInputsDto pagedInputsDto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<GetAppUserDto>> getClientReviews(PagedInputsDto pagedInputsDto)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetAppUserDto>> getProviderByServiceOrName(string searchKey)
        {
            var users =await _userManager.Users
                .Include(u=>u.Service)
                .Where(u => (u.Service.Title.Contains(searchKey) || u.FullName.Contains(searchKey))&&u.Type ==UserType.Provider)
                .OrderByDescending(u=>u.Rate).ToListAsync();

            var usersDto = _mapper.Map<List<GetAppUserDto>>(users);

            return usersDto;
        }

        public async Task<GetUserInfoForOrderDto> getUserInfoForOrderById(string id)
        {
            var client =await _userManager.Users
                .Include(u=>u.Service)
                .FirstOrDefaultAsync(u=>u.Id==id);

            if (client is null)
                return null;

            var clientDto = _mapper.Map<GetUserInfoForOrderDto>(client);

            return clientDto;
        }

        public async Task<bool> provideIsExists(string id)
        {
            var provider = await _userManager.Users.FirstOrDefaultAsync(u=>u.Id==id&&u.Type==UserType.Provider);
            
            return (provider is not null);
        }

        public async Task<bool> clientIsExists(string id)
        {
            var client = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id && u.Type == UserType.Client);

            return (client is not null);
        }
    }
}
