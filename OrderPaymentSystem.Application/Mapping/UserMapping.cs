using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Auth;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Mapping
{
    public class UserMapping:Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
