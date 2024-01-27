using Microsoft.AspNetCore.Http;

namespace ServicesApp.Domain.DTOs.ServiceDTOs
{
    public class CreateServiceDto
    {
        public string Title { get; set; } = null!;
        public IFormFile Image { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CreatedById { get; set; } = null!;
    }
}
