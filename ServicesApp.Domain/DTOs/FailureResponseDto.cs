namespace ServicesApp.Domain.DTOs
{
    public class FailureResponseDto
    {
        public int Status { get; set; }
        public string Message { get; set; } = null!;
    }
}
