namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = null!;

    }
}
