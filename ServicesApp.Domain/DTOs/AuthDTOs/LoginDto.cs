namespace ServicesApp.Domain.DTOs.AuthDTOs
{
    public class LoginDto
    {
        [Required, DataType(DataType.PhoneNumber)]
        public string UserName { get; set; } = null!;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public string FirebaseToken { get; set; } = null!;
    }
}
