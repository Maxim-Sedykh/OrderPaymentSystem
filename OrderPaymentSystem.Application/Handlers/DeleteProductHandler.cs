using MediatR;
using OrderPaymentSystem.Application.Commands.ProductCommands;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class DeleteProductHandler(IBaseRepository<Product> productRepository) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        productRepository.Remove(request.Product);
        await productRepository.SaveChangesAsync(cancellationToken);
    }
}
