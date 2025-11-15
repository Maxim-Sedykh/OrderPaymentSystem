using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Queries;

public class GetProductByIdQuery(int productId) : IRequest<ProductDto>
{
    public int ProductId { get; set; } = productId;
}
