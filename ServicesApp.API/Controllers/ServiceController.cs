using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Infrastructure.Consts;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.User)]
    public class ServiceController : ControllerBase
    {
        private readonly IServService _servService;
        public ServiceController(IServService servService)
        {
            _servService = servService;
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
