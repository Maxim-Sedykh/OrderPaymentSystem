using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Auth
{
    public record UserDto(string Login, string PhoneNumber, string Email);
}
