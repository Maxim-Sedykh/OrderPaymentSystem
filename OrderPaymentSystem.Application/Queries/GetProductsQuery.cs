using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Queries
{
    public class GetProductsQuery() : IRequest<ProductDto[]>
    {
    }
}
