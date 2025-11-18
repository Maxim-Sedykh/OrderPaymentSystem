using MediatR;
using OrderPaymentSystem.Domain.Dto.Product;

namespace OrderPaymentSystem.Application.Commands.ProductCommands;

public record CreateProductCommand(string ProductName, string Description, decimal Cost) : IRequest<ProductDto>;
