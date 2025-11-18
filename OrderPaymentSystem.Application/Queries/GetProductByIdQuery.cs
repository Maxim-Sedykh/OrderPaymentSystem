using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Queries;

public record GetProductByIdQuery(int ProductId) : IRequest<ProductDto>;
