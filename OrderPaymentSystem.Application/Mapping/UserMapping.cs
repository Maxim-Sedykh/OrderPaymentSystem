using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.Application.Mapping;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
