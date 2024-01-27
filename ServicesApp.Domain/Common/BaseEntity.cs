namespace ServicesApp.Domain.Common
{
    public class BaseEntity
    {
        [Required]
        public string CreatedById { get; set; } = null!;

        [DataType(DataType.DateTime), Required]
        public DateTime CreateOn { get; set; }
        public string? UpdatedById { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedOn { get; set; }
    }
}
