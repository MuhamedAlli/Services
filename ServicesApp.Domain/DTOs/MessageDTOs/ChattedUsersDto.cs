namespace ServicesApp.Domain.DTOs.MessageDTOs
{
    public class ChattedUsersDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Image { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageDate { get; set; }

    }
}
