using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicesApp.API.Helpers;
using ServicesApp.Domain.DTOs.AuthDTOs;
using ServicesApp.Infrastructure.Consts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twilio.Jwt.AccessToken;
using Twilio.Types;

namespace ServicesApp.Infrastructure.Persistence
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IMapper _mapper;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, IMapper mapper)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _mapper = mapper;
        }

        public async Task<AuthDto> RegisterStepOne(RegisterStepOneDto regiserStepOneDto)
        {
            if (await _userManager.FindByNameAsync(regiserStepOneDto.PhoneNumber) is not null)
                return new AuthDto { Message = "PhoneNumber is already registered!" };

            var user = new ApplicationUser
            {
                FullName = regiserStepOneDto.FullName,
                UserName = regiserStepOneDto.PhoneNumber,
                PhoneNumber = regiserStepOneDto.PhoneNumber,
                Type = regiserStepOneDto.UserType,
                ServiceId = regiserStepOneDto.ServiceId,
                CreateOn = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, regiserStepOneDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthDto { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, AppRoles.User);
            var jwtSecurityTaken = await CreateJwtToken(user);

            var appUser =await _userManager.Users.Include(u=>u.Service)
                               .FirstOrDefaultAsync(u=>u.PhoneNumber==regiserStepOneDto.PhoneNumber);
            var authDto = _mapper.Map<AuthDto>(appUser);
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityTaken);
            authDto.ExpiresOn = jwtSecurityTaken.ValidTo;
            authDto.IsSuccessful = true;
            authDto.Roles = new List<string> { AppRoles.User };

            return authDto;
        }
        public async Task<bool> RegisterStepTwo(string imageName, RegisterStepTwoDto registerStepTwoDto)
        {
            var user =await _userManager.FindByNameAsync(registerStepTwoDto.UserName);
            if (user is null)
                return false;

            user.Image = imageName;
            user.Bio = registerStepTwoDto.Bio;
            user.Address = registerStepTwoDto.Address;

           var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return true;

            return false;
        }
        public async Task<AuthDto> Login(LoginDto loginDto)
        {
            var authDto = new AuthDto();

            var user = await _userManager.Users.Include(u => u.Service)
                               .FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                authDto.Message = "Phone Number or Password is incorrect!";
                return authDto;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            //var appUser = await _userManager.Users.Include(u => u.Service)
            //                   .FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
            authDto = _mapper.Map<AuthDto>(user);
            
            authDto.IsSuccessful = true;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authDto.ExpiresOn = jwtSecurityToken.ValidTo;
            authDto.Roles = rolesList.ToList();

            user.FirebaseToken = loginDto.FirebaseToken;

            if(user.RefreshTokens.Any(t=> t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authDto.RefreshToken = activeRefreshToken!.Token;
                authDto.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authDto.RefreshToken = refreshToken.Token;
                authDto.RefreshTokenExpiration = refreshToken.ExpiresOn;

                //Save Refresh Token in database
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            await _userManager.UpdateAsync(user);
            return authDto;
        }

       /* public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }*/


        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public async Task<AuthDto> RefreshTokenAsync(string token)
        {
            var authDto = new AuthDto();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authDto.Message = "Invalid token";
                return authDto;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authDto.Message = "Inactive token";
                return authDto;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authDto.IsSuccessful = true;
            authDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authDto.UserName = user.UserName;
            authDto.PhoneNumber = user.PhoneNumber;
            authDto.IsVerified = user.IsVerified;
            var roles = await _userManager.GetRolesAsync(user);
            authDto.Roles = roles.ToList();
            authDto.RefreshToken = newRefreshToken.Token;
            authDto.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authDto;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}


