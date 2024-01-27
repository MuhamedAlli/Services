using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ServicesApp.API.Hubs;
using ServicesApp.Application.Interfaces;
using ServicesApp.Domain.Consts;
using ServicesApp.Domain.DTOs;
using ServicesApp.Domain.DTOs.MessageDTOs;

namespace ServicesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMessageService _messageService;
        private List<string> _AllowedExtensions = new() { ".jpeg", ".jpg", ".png", ".mp3", ".m4a", ".wav", ".mp4", ".3gp", ".mov", ".mkv" };
        private int _AllowedMaxSize = 2097152 * 3;
        public ChatController(IHubContext<ChatHub> hubContext, IWebHostEnvironment webHostEnvironment, IMessageService messageService)
        {
            _hubContext = hubContext;
            _webHostEnvironment = webHostEnvironment;
            _messageService = messageService;
        }
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromForm] CreateMessageDto createMessageDto)
        {
            if (createMessageDto.ContentUrl is not null)
            {
                var extensiont = Path.GetExtension(createMessageDto.ContentUrl.FileName);
                if (!_AllowedExtensions.Contains(extensiont))
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.validImageExtensions
                    });
                }
                if (createMessageDto.ContentUrl.Length > _AllowedMaxSize)
                {
                    return BadRequest(new FailureResponseDto
                    {
                        Status = BadRequest().StatusCode,
                        Message = ErrorMessages.maxImageLength
                    });
                }
                var contentName = $"{Guid.NewGuid()}{extensiont}";
                var contentPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/messages", contentName);
                using var stream = System.IO.File.Create(contentPath);
                createMessageDto.ContentUrl.CopyTo(stream);
                var result = await _messageService.SaveMessageInDb(contentName, createMessageDto);
                if (result is not null)
                {
                    var messageDto = _messageService.UpdateMessageStatus(result.Id);
                    await _hubContext.Clients
                        .Group(createMessageDto.ReceiverId)
                        .SendAsync("receiveNewMessage", messageDto);
                    return Ok(new SuccessResponseDto
                    {
                        Status = Ok().StatusCode,
                        Message = SuccessMessages.success,
                        Data = messageDto
                    });
                }
            }
            else
            {
                var result = await _messageService.SaveMessageInDb(null, createMessageDto);
                if (result is not null)
                {
                    var messageDto = _messageService.UpdateMessageStatus(result.Id);
                    await _hubContext.Clients
                        .Group(createMessageDto.ReceiverId)
                        .SendAsync("receiveNewMessage", messageDto);
                    return Ok(new SuccessResponseDto
                    {
                        Status = Ok().StatusCode,
                        Message = SuccessMessages.success,
                        Data = messageDto
                    });
                }
            }

            return BadRequest(new FailureResponseDto
            {
                Status = BadRequest().StatusCode,
                Message = ErrorMessages.failed
            });
        }
        [HttpGet("GetChatsForUser")]
        public async Task<IActionResult> GetChatsForUser([FromQuery] string userId)
        {
            var result = await _messageService.GetChatsForUser(userId);
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages([FromQuery] GetMessagesInputDto getMessagesInputDto)
        {
            var result = await _messageService.GetMessages(getMessagesInputDto);
            return Ok(new SuccessResponseDto
            {
                Status = Ok().StatusCode,
                Message = SuccessMessages.success,
                Data = result
            });
        }
    }
}
