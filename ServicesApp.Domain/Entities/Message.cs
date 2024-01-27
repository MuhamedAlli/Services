
namespace ServicesApp.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public MessageType Type { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public MessageState State { get; set; }
        [ForeignKey("Sender")]
        public string SenderId { get; set; } = null!;
        public ApplicationUser Sender { get; set; }
        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; } = null!;
        public ApplicationUser Receiver { get; set; }

        // Additional properties for different message types
        public string? ContentUrl { get; set; }
    }
}
