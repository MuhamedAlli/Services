using Microsoft.AspNetCore.Http;

namespace ServicesApp.Domain.DTOs.ServiceDTOs
{
    public class EditServiceDto
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string UpdatedById { get; set; } = null!;
    }
}
