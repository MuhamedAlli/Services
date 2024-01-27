using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Domain.DTOs.AccountDTOs
{
    public class GetUserInfoForOrderDto
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? Address { get; set; }
        public string? ServiceType { get; set; }
        public decimal VisitPrice { get; set; }

    }
}
