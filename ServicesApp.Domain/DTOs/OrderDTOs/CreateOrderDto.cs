
namespace ServicesApp.Domain.DTOs.OrderDTOs
{
    public class CreateOrderDto
    {
        [Required]
        public string CreatedById { get; set; } = null!;
        [Required]
        public string ProviderId { get; set; } = null!;
        [Required]
        public string OrderTitle { get; set; } = null!;
        [Required]
        public string OrderDescription { get; set; } = null!;
        [Required]
        public string OrderAddress { get; set; } = null!;

        [DataType(DataType.DateTime),Required]
        public DateTime OrderDate { get; set; }

        [DataType(DataType.Time),Required]
        public string OrderTime { get; set; } = null!;
        [Required]
        public decimal VisitPrice { get; set; }
    }
}
