using Microsoft.AspNetCore.Mvc;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.DTOs.AuthDTOs;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAccountService _accountService;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;

        private readonly List<string> _AllowedExtensions = new() { ".jpeg", ".jpg", ".png" };
        private readonly int _AllowedMaxSize = 2097152;
        public AccountController(IWebHostEnvironment webHostEnvironment, IAccountService accountService, IOtpService otpService, ISmsService smsService)
        {
            _webHostEnvironment = webHostEnvironment;
            _accountService = accountService;
            _otpService = otpService;
            _smsService = smsService;
        }

        [HttpPost("ChangeUserImage")]
        public async Task<IActionResult> ChangeUserImage([FromForm] ChangeUserImageDto changeUserImageDto)
        {
            if (changeUserImageDto.Image is null)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.imageFeildRequired
                });

            if (string.IsNullOrEmpty(changeUserImageDto.ApplicationUserId))
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.appUserIdRequired
                });

            var existedImageName = await _accountService.getExitedImageName(changeUserImageDto);
            if (existedImageName is not null)
            {
                var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/Users", existedImageName);
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
            var extensiont = Path.GetExtension(changeUserImageDto.Image.FileName);
            if (!_AllowedExtensions.Contains(extensiont))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.validImageExtensions
                });
            }
            if (changeUserImageDto.Image.Length > _AllowedMaxSize)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.maxImageLength
                });
            }
            var imageName = $"{Guid.NewGuid()}{extensiont}";
            var imagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/Users", imageName);
            using var stream = System.IO.File.Create(imagePath);
            changeUserImageDto.Image.CopyTo(stream);

            var result = await _accountService.changeImage(imageName, changeUserImageDto);

            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.somthingWentWrong
                });
            }
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = "Success",
                Data = new
                {
                    Image = result
                }
            });
        }

        [HttpPost("ChangeFullName")]
        public async Task<IActionResult> ChangeFullName(ChangeFullNameDto changeFullNameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ModelState.ToString()!
                });
            }
            var result = await _accountService.changeFullName(changeFullNameDto);
            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = new
                {
                    FullName = result
                }
            });
        }
        [HttpPost("ChangeAddress")]
        public async Task<IActionResult> ChangeAddress(ChangeAddressDto changeAddressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            var result = await _accountService.changeAddress(changeAddressDto);
            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = new
                {
                    Address = result
                }
            });
        }
        [HttpPost("ChangeBio")]
        public async Task<IActionResult> ChangeBio(ChangeBioDto changeBioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            var result = await _accountService.changeBio(changeBioDto);
            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = new
                {
                    Bio = result
                }
            });
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }

            var result = await _accountService.changePassword(changePasswordDto);

            if (!result)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.somthingWentWrong
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success
            });

        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            var result = await _accountService.forgetPassword(forgetPasswordDto);
            if (!result)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.userNotFound
                });
            }
            //Generate OTP 
            string otpCode = _otpService.GenerateOtp();
            var message = $"Your verification code is : {otpCode}";
            OtpDto otpDto = new()
            {
                PhoneNumber = forgetPasswordDto.PhoneNumber,
                OtpCode = otpCode,
            };
            _otpService.StoreOtp(otpDto);
            //Send OTP To PhoneNumber
            var res = await _smsService.SendSmsAsync(forgetPasswordDto.PhoneNumber, message);

            if (!res)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success
            });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }

            var result = await _accountService.resetPassword(resetPasswordDto);
            if (!result)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.somthingWentWrong
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success
            });
        }

        [HttpPost("ChangePhone")]
        public async Task<IActionResult> ChangePhone(ChangePhoneDto changePhoneDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            var result = await _accountService.changePhone(changePhoneDto);

            if (!result)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.somthingWentWrong
                });
            }
            //Generate OTP 
            string otpCode = _otpService.GenerateOtp();
            var message = $"Your verification code is : {otpCode}";
            OtpDto otpDto = new()
            {
                PhoneNumber = changePhoneDto.PhoneNumber,
                OtpCode = otpCode,
            };
            _otpService.StoreOtp(otpDto);
            //Send OTP To PhoneNumber
            var res = await _smsService.SendSmsAsync(changePhoneDto.PhoneNumber, message);

            if (!res)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.somthingWentWrong
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = new
                {
                    PhoneNumber = changePhoneDto.PhoneNumber,
                }
            });
        }
        //Get User By Id 
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.failed
                });
            }
            var result = await _accountService.getUserById(id);
            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.userNotFound
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetTopRatedProviders")]
        public IActionResult GetTopRatedProviders()
        {
            var result = _accountService.getTopRatedProviders();

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });

        }
        [HttpGet("GetTopRatedClients")]
        public IActionResult GetTopRatedClients()
        {
            var result = _accountService.getTopRatedClients();
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });

        }
        [HttpGet("GetAllProviders")]
        public async Task<IActionResult> GetAllProviders([FromQuery] PagedInputsDto pagedInputsDto)
        {
            if (pagedInputsDto.Page <= 0 || pagedInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }

            var result = await _accountService.getAllProviders(pagedInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetAllClients")]
        public async Task<IActionResult> GetAllClients([FromQuery] PagedInputsDto pagedInputsDto)
        {
            if (pagedInputsDto.Page <= 0 || pagedInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }

            var result = await _accountService.getAllClients(pagedInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetProviderByServiceOrName")]
        public async Task<IActionResult> GetProviderByServiceOrName([FromQuery] string searchKey)
        {
            if (!string.IsNullOrEmpty(searchKey))
            {
                var result = await _accountService.getProviderByServiceOrName(searchKey);
                return Ok(new SuccessResponseDto
                {
                    Status = Ok().StatusCode,
                    Message = SuccessMessages.success,
                    Data = result
                });
            }
            return BadRequest(new FailureResponseDto
            {
                Status = BadRequest().StatusCode,
                Message = ErrorMessages.invalidInput
            });
        }
        [HttpGet("GetUserInfoToCreateOrder")]
        public async Task<IActionResult> GetUserInfoToCreateOrder(string id)
        {
            var result = await _accountService.getUserInfoForOrderById(id);
            if (result is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.userNotFound
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });

        }
    }
}
