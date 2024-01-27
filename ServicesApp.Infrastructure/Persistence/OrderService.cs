using AutoMapper;
using ServicesApp.Domain.DTOs.OrderDTOs;
using ServicesApp.Domain.Entities;
using ServicesApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesApp.Infrastructure.Persistence
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountService _accountService;

        public OrderService(IMapper mapper, ApplicationDbContext dbContext, IAccountService accountService)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _accountService = accountService;
        }

      
        public async Task<OrderResponseDto> CreateOrder(CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            order.CreateOn=DateTime.Now;
            order.State = OrderSatet.Pending;
            _dbContext.Orders.Add(order);

            var result =  await _dbContext.SaveChangesAsync();

            if (result <= 0)
                return null;

            var providerInfo =await _accountService.getUserInfoForOrderById(createOrderDto.ProviderId);
            var orderDto = _mapper.Map<OrderResponseDto>(order);
            orderDto.PrviderName = providerInfo.FullName;
            orderDto.PrviderPhone = providerInfo.UserName;
            orderDto.ServiceType = providerInfo.ServiceType;
            orderDto.VisitPrice = providerInfo.VisitPrice;
            return orderDto;
        }
        public async Task<OrderResponseDto> AcceptOrder(AccepOrderDto accepOrderDto)
        {
            var pendingOrder =await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == accepOrderDto.OrderId);
            var provider = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == accepOrderDto.ProviderId);

            if (pendingOrder is null || provider is null)
                return null;

            //Update order status 
            pendingOrder.State = OrderSatet.Accepted;
            pendingOrder.UpdatedOn=DateTime.Now;
            pendingOrder.UpdatedById = accepOrderDto.ProviderId;

            //Update provider info with new accepted order
            provider.UnCompletedTasks = ++provider.UnCompletedTasks;
            var result =await _dbContext.SaveChangesAsync();
            var providerInfo = await _accountService.getUserInfoForOrderById(accepOrderDto.ProviderId);
            
            var orderDto = _mapper.Map<OrderResponseDto>(pendingOrder);
            
            orderDto.PrviderName = providerInfo.FullName;
            orderDto.PrviderPhone = providerInfo.UserName;
            orderDto.ServiceType = providerInfo.ServiceType;
            orderDto.VisitPrice = providerInfo.VisitPrice;
            
            return orderDto;

        }


        public async Task<OrderResponseDto> RejectOrder(ChangeOrderStateDto changeOrderStateDto)
        {
            var result = await ChangeOrderState(OrderSatet.Rejected,changeOrderStateDto);
            return result;
        }

        public async Task<OrderResponseDto> CanceledOrder(ChangeOrderStateDto changeOrderStateDto)
        {
            var result = await ChangeOrderState(OrderSatet.Canceled, changeOrderStateDto);
            return result;
        }

        private async Task<OrderResponseDto> ChangeOrderState(OrderSatet orderState, ChangeOrderStateDto changeOrderStateDto)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == changeOrderStateDto.OrderId);
            var provider = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == changeOrderStateDto.ProviderId);

            if (order is null || provider is null)
                return null;

            //Update order status 
            order.State = orderState;
            order.UpdatedOn = DateTime.Now;
            order.UpdatedById = changeOrderStateDto.ProviderId;
            order.Reason = changeOrderStateDto.Reason;
            //Update provider info with new accepted order
            //provider.RejectedTasks = ++provider.RejectedTasks;
            switch (orderState)
            {
                case OrderSatet.Rejected:
                    provider.RejectedTasks = ++provider.RejectedTasks;
                    break;
                case OrderSatet.Canceled:
                    provider.UnCompletedTasks = --provider.UnCompletedTasks;
                    provider.CanceledTasks = ++provider.CanceledTasks;
                    break;

            }
            var result = await _dbContext.SaveChangesAsync();

            if (result <= 0)
                return null;

            var providerInfo = await _accountService.getUserInfoForOrderById(changeOrderStateDto.ProviderId);

            var orderDto = _mapper.Map<OrderResponseDto>(order);

            orderDto.PrviderName = providerInfo.FullName;
            orderDto.PrviderPhone = providerInfo.UserName;
            orderDto.ServiceType = providerInfo.ServiceType;
            orderDto.VisitPrice = providerInfo.VisitPrice;
            orderDto.Reason = changeOrderStateDto.Reason;

            return orderDto;

        }
    }
}
