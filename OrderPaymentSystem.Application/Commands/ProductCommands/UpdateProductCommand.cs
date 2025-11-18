using MediatR;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Commands.ProductCommands;

public record UpdateProductCommand(string ProductName, string Description, decimal Cost, Product Product) : IRequest;
