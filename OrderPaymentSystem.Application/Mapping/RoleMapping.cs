using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.Application.Mapping;

public class RoleMapping : Profile
{
    public RoleMapping()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}
