namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class GetUserReviewsDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Service { get; set; }
        public string? Image { get; set; }
        public decimal? Rate { get; set; }
        public string? Comment { get; set; }
    }
}
