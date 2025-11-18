using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Queries;

public record GetProductsQuery() : IRequest<ProductDto[]>;
