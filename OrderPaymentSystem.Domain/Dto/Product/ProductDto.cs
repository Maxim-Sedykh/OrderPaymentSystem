using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Product
{
    public record ProductDto(long Id, string ProductName, string Description, decimal Cost, string CreatedAt);
}
