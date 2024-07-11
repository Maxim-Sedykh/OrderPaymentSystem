using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Commands
{
    public record CreateProductCommand(string ProductName, string Description, decimal Cost): IRequest<ProductDto>;
}
