namespace ServicesApp.Domain.DTOs.ServiceDTOs
{
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Description { get; set; } = null!;
        public string CreatedById { get; set; } = null!;
        public DateTime CreateOn { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
