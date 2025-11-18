using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Payment;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Mapping;

public class PaymentMapping : Profile
{
    public PaymentMapping()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BasketId, opt => opt.MapFrom(src => src.BasketId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.AmountOfPayment, opt => opt.MapFrom(src => src.AmountOfPayment))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.CashChange, opt => opt.MapFrom(src => src.CashChange))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.DeliveryAddress.Street))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.DeliveryAddress.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.DeliveryAddress.Country))
            .ForMember(dest => dest.Zipcode, opt => opt.MapFrom(src => src.DeliveryAddress.ZipCode));
    }
}
