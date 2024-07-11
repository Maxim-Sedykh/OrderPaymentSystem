using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Product
{
    public record CreateProductDto(string ProductName, string Description, decimal Cost);
}
