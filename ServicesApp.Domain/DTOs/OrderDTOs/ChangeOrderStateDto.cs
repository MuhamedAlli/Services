
namespace ServicesApp.Domain.DTOs.OrderDTOs
{
    public class ChangeOrderStateDto
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public string ProviderId { get; set; } = null!;

        [MinLength(8), Required]
        public string Reason { get; set; } = null!;
    }
}
