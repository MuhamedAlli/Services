

using Microsoft.AspNetCore.Http;

namespace ServicesApp.Domain.DTOs.MessageDTOs
{
    public class CreateMessageDto
    {
        public MessageType Type { get; set; }
        public string? Content { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public IFormFile? ContentUrl { get; set; }
    }
}
