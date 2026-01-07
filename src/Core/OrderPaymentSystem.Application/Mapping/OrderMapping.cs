using AutoMapper;
using OrderPaymentSystem.Application.DTOs.Order;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Mapping;

public class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<OrderItem, OrderDto>()
            .ReverseMap();
    }
}
