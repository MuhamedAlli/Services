using Microsoft.AspNetCore.Http;

namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ChangeUserImageDto
    {
        public string ApplicationUserId { get; set; } = null!;

        public IFormFile Image { get; set; } = null!;
    }
}
