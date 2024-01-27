namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ChangeFullNameDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;
        [StringLength(maximumLength: 50, ErrorMessage = "FullName must be less than 50 and greater than 5 characters!", MinimumLength = 5)]
        public string FullName { get; set; } = null!;
    }
}
