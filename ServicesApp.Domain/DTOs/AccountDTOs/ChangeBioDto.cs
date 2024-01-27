namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class ChangeBioDto
    {
        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [StringLength(maximumLength: 150, ErrorMessage = "Bio must be less than {1} and greater than {2} characters!", MinimumLength = 5)]
        public string Bio { get; set; } = null!;
    }
}
