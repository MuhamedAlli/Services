namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ChangeAddressDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;
        [StringLength(maximumLength: int.MaxValue, ErrorMessage = "Address must be less than 50 and greater than 5 characters!", MinimumLength = 5)]
        public string Address { get; set; } = null!;
    }
}
