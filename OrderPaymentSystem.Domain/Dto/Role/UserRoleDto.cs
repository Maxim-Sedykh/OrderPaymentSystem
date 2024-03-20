using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Role
{
    public record UserRoleDto(
            string Login,
            string RoleName
        );
}
