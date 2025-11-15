using MediatR;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.Application.Commands;

public record UpdateProductCommand(string ProductName, string Description, decimal Cost, Product Product) : IRequest;
