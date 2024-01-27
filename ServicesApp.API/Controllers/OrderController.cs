using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.OrderDTOs;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;

        public OrderController(IOrderService orderService, IAccountService accountService)
        {
            _orderService = orderService;
            _accountService = accountService;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody]CreateOrderDto createOrderDto)
        {
            if(! await _accountService.provideIsExists(createOrderDto.ProviderId))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.providerNotFound
                });
            }

            if (!await _accountService.clientIsExists(createOrderDto.CreatedById))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.clientNotFound
                });
            }

            var data =await _orderService.CreateOrder(createOrderDto);
            if (data is null)
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
        [HttpPost("AcceptOrder")]
        public async Task<IActionResult> AcceptOrder(AccepOrderDto accepOrderDto)
        {
            if (!await _accountService.provideIsExists(accepOrderDto.ProviderId))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.providerNotFound
                });
            }
            var data = await _orderService.AcceptOrder(accepOrderDto);
            if (data is null)
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
        [HttpPost("RejectOrder")]
        public async Task<IActionResult> RejectOrder(ChangeOrderStateDto changeOrderStateDto)
        {
            if (!await _accountService.provideIsExists(changeOrderStateDto.ProviderId))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.providerNotFound
                });
            }
            var data = await _orderService.RejectOrder(changeOrderStateDto);
            if (data is null)
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
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(ChangeOrderStateDto changeOrderStateDto)
        {
            if (!await _accountService.provideIsExists(changeOrderStateDto.ProviderId))
            {
                return BadRequest(new FailureResponseDto
                {
                    Status = BadRequest().StatusCode,
                    Message = ErrorMessages.providerNotFound
                });
            }
            var data = await _orderService.CanceledOrder(changeOrderStateDto);
            if (data is null)
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
    }
}
