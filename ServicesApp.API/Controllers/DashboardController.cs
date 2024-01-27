using Microsoft.AspNetCore.Mvc;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.ServiceDTOs;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashService _dashService;
        private readonly IServService _servService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly List<string> _AllowedExtensions = new() { ".jpeg", ".jpg", ".png" };
        private readonly int _AllowedMaxSize = 2097152;


        public DashboardController(IDashService dashService, IServService servService, IWebHostEnvironment webHostEnvironment)
        {
            _dashService = dashService;
            _servService = servService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet("GetStatistics")]
        public IActionResult GetStatistics()
        {
            var data = _dashService.GetStatistics();
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = data
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

            var result = await _dashService.getAllProviders(pagedInputsDto);

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

            var result = await _dashService.getAllClients(pagedInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetClientByNameOrPhone")]
        public async Task<IActionResult> GetClientByNameOrPhone([FromQuery] PagedSearchInputsDto pagedSearchInputsDto)
        {
            if (pagedSearchInputsDto.Page <= 0 || pagedSearchInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }

            var result = await _dashService.getClientByNameOrPhone(pagedSearchInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetProviderByNameOrPhone")]
        public async Task<IActionResult> GetProviderByNameOrPhone([FromQuery] PagedSearchInputsDto pagedSearchInputsDto)
        {
            if (pagedSearchInputsDto.Page <= 0 || pagedSearchInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }

            var result = await _dashService.getProviderByNameOrPhone(pagedSearchInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetAllAdmins")]
        public async Task<IActionResult> GetAllAdmins([FromQuery] PagedInputsDto pagedInputsDto)
        {
            if (pagedInputsDto.Page <= 0 || pagedInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }

            var result = await _dashService.getAllAdmins(pagedInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await _dashService.getAllRoles();
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = data
            });
        }
        //Actions OF Service Entity


        [HttpPost("CreateService")]
        public async Task<ActionResult> CreateService([FromForm] CreateServiceDto createServiceDto)
        {
            //createServiceDto.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (createServiceDto.Image is not null)
            {
                var extensiont = Path.GetExtension(createServiceDto.Image.FileName);
                if (!_AllowedExtensions.Contains(extensiont))
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.validImageExtensions
                    });
                }
                if (createServiceDto.Image.Length > _AllowedMaxSize)
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.maxImageLength
                    });
                }
                var imageName = $"{Guid.NewGuid()}{extensiont}";
                var imagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/services", imageName);
                using var stream = System.IO.File.Create(imagePath);

                createServiceDto.Image.CopyTo(stream);

                var result = await _servService.CreateService(imageName, createServiceDto);
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
                    Message = SuccessMessages.success,
                    Data = result
                });
            }
            return BadRequest(new FailureResponseDto
            {
                Status = BadRequest().StatusCode,
                Message = ErrorMessages.failed
            });
        }
        [HttpPost("EditService")]
        public async Task<IActionResult> EditService([FromForm] EditServiceDto editServiceDto)
        {
            if (editServiceDto.Image is null)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.imageFeildRequired
                });

            if (editServiceDto.Id <= 0)
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.serviceIdRequired
                });

            var existedImageName = _servService.getExitedImageName(editServiceDto);
            if (existedImageName is not null)
            {
                var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/services", existedImageName);
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }
            var extensiont = Path.GetExtension(editServiceDto.Image.FileName);
            if (!_AllowedExtensions.Contains(extensiont))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.validImageExtensions
                });
            }
            if (editServiceDto.Image.Length > _AllowedMaxSize)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.maxImageLength
                });
            }
            var imageName = $"{Guid.NewGuid()}{extensiont}";
            var imagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/services", imageName);
            using var stream = System.IO.File.Create(imagePath);
            editServiceDto.Image.CopyTo(stream);

            var result = await _servService.EditService(imageName, editServiceDto);

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
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpDelete("DeleteService")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var data = _servService.GetServiceById(id);
            var result = await _servService.DeleteService(id);
            if (data is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.serviceNotFound
                });
            }
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
                Message = SuccessMessages.success,
                Data = data
            });
        }
        [HttpGet("GetServiceById")]
        public IActionResult GetServiceById(int id)
        {
            var data = _servService.GetServiceById(id);

            if (data is null)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.serviceNotFound
                });
            }

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = data
            });
        }
        [HttpGet("GetServicesByTitle")]
        public async Task<IActionResult> GetServicesByTitle(string title)
        {
            var data = await _servService.GetServicesByTitle(title);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = data
            });
        }
        [HttpGet("GetAllServices")]
        public async Task<IActionResult> GetAllServices([FromQuery] PagedInputsDto pagedInputsDto)
        {
            if (pagedInputsDto.Page <= 0 || pagedInputsDto.PageSize <= 0)
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.invalidPageOrPageSize
                });
            }
            var data = await _servService.GetAllServices(pagedInputsDto);

            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = data
            });
        }
    }
}
