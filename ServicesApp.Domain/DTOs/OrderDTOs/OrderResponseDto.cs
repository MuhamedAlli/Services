namespace ServicesApp.Domain.DTOs.OrderDTOs
{
    public class OrderResponseDto
    {
        public string ServiceType { get; set; } = null!;
        public string PrviderName { get; set; } = null!;
        public string PrviderPhone { get; set; } = null!;
        public int Id { get; set; }
        public int State { get; set; }
        public string OrderTitle { get; set; } = null!;
        public string OrderDescription { get; set; } = null!;
        public string OrderAddress { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string OrderTime { get; set; } = null!;
        public decimal VisitPrice { get; set; }
        public decimal MaterialsPrice { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Reason { get; set; } = null!;

    }
}
