using ServicesApp.Domain.DTOs.OrderDTOs;
using ServicesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Application.Interfaces
{
    public interface IOrderService
    {
        public Task<OrderResponseDto> CreateOrder(CreateOrderDto createOrderDto);
        public Task<OrderResponseDto> AcceptOrder(AccepOrderDto accepOrderDto);
        public Task<OrderResponseDto> RejectOrder(ChangeOrderStateDto changeOrderStateDto);
        public Task<OrderResponseDto> CanceledOrder(ChangeOrderStateDto changeOrderStateDto);

    }
}
