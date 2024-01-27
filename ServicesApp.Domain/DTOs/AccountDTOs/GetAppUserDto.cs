
namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class GetAppUserDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string Type { get; set; } = null!;
        public bool IsVerified { get; set; }
        public string? State { get; set; }
        public bool IsAvailable { get; set; }
        public decimal CostHour { get; set; }
        public decimal Balance { get; set; }
        public decimal PendingMoney { get; set; }
        public int CompletedTasks { get; set; }
        public int UnCompletedTasks { get; set; }
        public int RejectedTasks { get; set; }
        public int CanceledTasks { get; set; }
        public decimal Rate { get; set; }
        public string? Service { get; set; }
        public List<string> Roles { get; set; }
    }
}
