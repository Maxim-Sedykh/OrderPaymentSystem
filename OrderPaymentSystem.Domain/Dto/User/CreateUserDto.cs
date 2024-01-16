using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.User
{
    public record CreateUserDto(string Name, string Surname, string Patronymic,
        string Login, string Password, string PhoneNumber, string Email);
}
