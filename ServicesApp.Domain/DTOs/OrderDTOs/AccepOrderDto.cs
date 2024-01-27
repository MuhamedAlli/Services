using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Domain.DTOs.OrderDTOs
{
    public class AccepOrderDto
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string ProviderId { get; set; } = null!;
    }
}
