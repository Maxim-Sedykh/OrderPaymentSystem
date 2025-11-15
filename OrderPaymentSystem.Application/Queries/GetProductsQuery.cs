using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Queries;

public class GetProductsQuery() : IRequest<ProductDto[]>
{
}
