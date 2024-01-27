namespace ServicesApp.Domain.DTOs
{
    public class SuccessResponseDto
    {
        public int Status { get; set; }
        public string Message { get; set; } = null!;
        public object Data { get; set; } = null!;
    }
}
