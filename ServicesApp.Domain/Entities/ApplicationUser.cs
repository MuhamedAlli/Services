
using Microsoft.EntityFrameworkCore;
using ServicesApp.Domain.Enums;

namespace ServicesApp.Domain.Entities
{
    [Index(nameof(UserName),IsUnique =true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class ApplicationUser :IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string? Address { get; set; } 
        public DateTime? BirthDate { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public UserType? Type { get; set; }
        public bool IsVerified { get; set; }
        public UserState? State { get; set; } = UserState.Pending;
        public bool IsAvailable { get; set; }
        public decimal VisitPrice { get; set; }
        public decimal Balance { get; set; }
        public decimal PendingMoney { get; set; }
        public int CompletedTasks { get; set; }
        public int UnCompletedTasks { get; set; }
        public int RejectedTasks { get; set; }
        public int CanceledTasks { get; set; }
        public decimal Rate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string? FirebaseToken { get; set; } 

        //Relationships 
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
        [ForeignKey("Service")]
        public int? ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
