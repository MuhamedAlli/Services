using AutoMapper;
using ServicesApp.Domain.DTOs.AccountDTOs;
using ServicesApp.Domain.DTOs.AuthDTOs;
using ServicesApp.Domain.DTOs.MessageDTOs;
using ServicesApp.Domain.DTOs.OrderDTOs;
using ServicesApp.Domain.DTOs.ServiceDTOs;
using ServicesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account.Sip.Domain.AuthTypes.AuthTypeCalls;

namespace ServicesApp.Domain.Common
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            //Users
            CreateMap<RegisterStepOneDto, ApplicationUser>().ReverseMap();
            CreateMap<ApplicationUser, AuthDto>()
                .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service.Title));

            CreateMap<ApplicationUser, GetAppUserDto>()
                .ForMember(dest=>dest.Service, opt=>opt.MapFrom(src=>src.Service.Title));

            CreateMap<ApplicationUser, GetUserInfoForOrderDto>()
                .ForMember(dest=>dest.ServiceType, opt=>opt.MapFrom(src=>src.Service.Title));
            
            //Otp
            CreateMap<OtpDto, Otp>().ReverseMap();

            //Services
            CreateMap<CreateServiceDto, Service> ();

            CreateMap<Service, ServiceResponseDto>().ReverseMap();

            //Messages
            CreateMap<Message, CreateMessageDto>().ReverseMap();

            CreateMap<Message, MessageResponseDto>();

            //Orders
            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order, OrderResponseDto>();
        }
    }
}
