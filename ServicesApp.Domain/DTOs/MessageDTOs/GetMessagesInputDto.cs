namespace ServicesApp.Domain.DTOs.MessageDTOs
{
    public class GetMessagesInputDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
    }
}
