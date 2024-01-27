
namespace ServicesApp.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int Id { get; set; }
        public string OrderTitle { get; set; } = null!;
        public string OrderDescription { get; set; } = null!;
        public string OrderAddress { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [DataType(DataType.Time)]
        public string OrderTime { get; set; } = null!;
        public decimal VisitPrice { get; set; }
        public decimal MaterialsPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("ProvidedBy")]
        public string ProviderId { get; set; } = null!;
        public ApplicationUser ProvidedBy { get; set; }
        public DateTime? AcceptedOn { get; set; }
        public decimal ProviderRate { get; set; }
        public string? ProviderComment { get; set; }
        public decimal ClientRate { get; set; }
        public string? ClientComment { get; set; }
        public OrderSatet State { get; set; }
        public string? Reason { get; set; }
    }
}
