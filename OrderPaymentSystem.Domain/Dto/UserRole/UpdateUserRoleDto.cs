using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.UserRole
{
    public record UpdateUserRoleDto(
            string Login,
            long FromRoleId,
            long ToRoleId
        );
}
