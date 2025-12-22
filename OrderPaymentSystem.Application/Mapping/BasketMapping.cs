using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Basket;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Mapping;

public class BasketMapping : Profile
{
    public BasketMapping()
    {
        CreateMap<BasketItem, BasketDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToLongDateString()))
            .ForMember(dest => dest.CostOfAllOrders, opt => opt.MapFrom(src => src.Orders.Sum(x => x.OrderCost)))
            .ReverseMap();
    }
}
