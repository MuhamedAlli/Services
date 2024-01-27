using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AuthDTOs;
using ServicesApp.Domain.Entities;
using System.Diagnostics;
using Twilio.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISmsService _smsService;
        private readonly IOtpService _otpService;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private List<string> _AllowedExtensions = new() { ".jpeg", ".jpg", ".png" };
        private int _AllowedMaxSize = 2097152;

        public AuthController(IOtpService otpService, ISmsService smsService, IAuthService authService, IWebHostEnvironment webHostEnvironment)
        {
            _otpService = otpService;
            _smsService = smsService;
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("RegisterStepOne")]
        public async Task<IActionResult> RegisterStepOne([FromBody] RegisterStepOneDto registerStepOneDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailureResponseDto
                {
                    Status =BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            //Create register new user with taken
            var data =await _authService.RegisterStepOne(registerStepOneDto);

            if (data.IsSuccessful)
            {
                //Generate OTP 
                string otpCode = _otpService.GenerateOtp();
                var message = $"Your verification code is : {otpCode}";
                OtpDto otpDto = new()
                {
                    PhoneNumber = registerStepOneDto.PhoneNumber,
                    OtpCode = otpCode,
                };
                _otpService.StoreOtp(otpDto);
                //Send OTP To PhoneNumber
                var result = await _smsService.SendSmsAsync(registerStepOneDto.PhoneNumber, message);
                //Debug.WriteLine(result.ToString());
                if (!string.IsNullOrEmpty(data.RefreshToken))
                    SetRefreshTokenInCookie(data.RefreshToken, data.RefreshTokenExpiration);

                return Ok(new SuccessResponseDto
                {
                    Status=Ok().StatusCode,
                    Message =SuccessMessages.success,
                    Data = data
                });
            }
            else
            {
                return BadRequest(new
                {
                    Status = BadRequest().StatusCode,
                    Message = data.Message
                });
            }
            
        }
        [HttpPost("VerifyOTP")]
        public  IActionResult VerifyOTP(VerifyOTPDto verifyOTPDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailureResponseDto
                {
                    Status=BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });

            var result = _otpService.ValidateOtp(verifyOTPDto);

            if (!result)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success
            });
        }
        [HttpPost("RegisterStepTwo")]
        public async Task<IActionResult> RegisterStepTwo([FromForm] RegisterStepTwoDto registerStepTwoDto)
        {
            if(registerStepTwoDto.Image is not null)
            {
                var extensiont = Path.GetExtension(registerStepTwoDto.Image.FileName);
                if (!_AllowedExtensions.Contains(extensiont))
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.validImageExtensions
                    });
                }
                if (registerStepTwoDto.Image.Length > _AllowedMaxSize)
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.maxImageLength
                    });
                }
                var imageName = $"{Guid.NewGuid()}{extensiont}";
                var imagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/users", imageName);
                using var stream = System.IO.File.Create(imagePath);
                registerStepTwoDto.Image.CopyTo(stream);
                var result =await _authService.RegisterStepTwo(imageName , registerStepTwoDto);
                if (result)
                    return Ok(new SuccessResponseDto
                    {
                        Status = Ok().StatusCode,
                        Message = SuccessMessages.success
                    });
            }

            return BadRequest(new FailureResponseDto
            {
                Status = BadRequest().StatusCode,
                Message = ErrorMessages.failed
            });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }

            var result  =await _authService.Login(loginDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = result.Message
                });
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });

        }
        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken!);

            if (!result.IsSuccessful)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = result.Message
                });

            SetRefreshTokenInCookie(result.RefreshToken!, result.RefreshTokenExpiration);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto revokeTokenDto)
        {
            var token = revokeTokenDto.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.tokenIsRequird
                });

            var result =await _authService.RevokeTokenAsync(token);
            if(!result)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.tokenInvalid
                });

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success
            });
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
