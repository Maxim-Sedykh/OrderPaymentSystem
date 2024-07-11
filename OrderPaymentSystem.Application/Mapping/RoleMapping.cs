using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Dto.Role;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
