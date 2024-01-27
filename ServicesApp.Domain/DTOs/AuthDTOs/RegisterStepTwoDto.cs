using Microsoft.AspNetCore.Http;

namespace ServicesApp.Domain.DTOs.AuthDTOs
{
    public class RegisterStepTwoDto
    {
        public string UserName { get; set; } = null!;
        public IFormFile? Image { get; set; }
        public string? Address { get; set; }
        public string? Bio { get; set; }
    }
}
