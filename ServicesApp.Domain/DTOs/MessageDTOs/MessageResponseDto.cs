
namespace ServicesApp.Domain.DTOs.MessageDTOs
{
    public class MessageResponseDto
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public int State { get; set; }
        public string? ContentUrl { get; set; }
    }
}
