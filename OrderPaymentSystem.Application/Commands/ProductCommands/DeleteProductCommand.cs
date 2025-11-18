using MediatR;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Commands.ProductCommands;

public record DeleteProductCommand(Product Product) : IRequest;
