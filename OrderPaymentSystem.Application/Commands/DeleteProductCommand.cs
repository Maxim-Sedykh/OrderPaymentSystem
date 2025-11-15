using MediatR;
using OrderPaymentSystem.Domain.Entity;

namespace OrderPaymentSystem.Application.Commands;

public record DeleteProductCommand(Product Product) : IRequest;
