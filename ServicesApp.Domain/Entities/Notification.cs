namespace ServicesApp.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsReaded { get; set; }
        public DateTime CreatedOn { get; set; }
        public NotificationType Type { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; }
    }
}
