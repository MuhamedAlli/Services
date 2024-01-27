namespace ServicesApp.Domain.DTOs.AuthDTOs
{
    public class RegisterStepOneDto
    {
        [Required, StringLength(maximumLength: 100, ErrorMessage = "{0} Length must be greater than {2} and less than {1}", MinimumLength = 5)]
        public string FullName { get; set; } = null!;
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^((\+20|0020|0)?1[0-9]{9})$|^1[0-9]{10}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = null!;
        [DataType(DataType.Password), Compare("ConfirmPassword")]
        public string Password { get; set; } = null!;
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
        public UserType UserType { get; set; }
        public int? ServiceId { get; set; }
    }
}
