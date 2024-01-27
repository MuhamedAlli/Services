
namespace ServicesApp.Domain.Entities
{
    public class Service : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Description { get; set; } = null!;
        public ICollection<ApplicationUser>? ApplicationUsers { get; set; }
    }
}
