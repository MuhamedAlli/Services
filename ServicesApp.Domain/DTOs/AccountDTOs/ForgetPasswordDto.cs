namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ForgetPasswordDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^((\+20|0020|0)?1[0-9]{9})$|^1[0-9]{10}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = null!;
    }
}
