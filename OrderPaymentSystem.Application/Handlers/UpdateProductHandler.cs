using MediatR;
using OrderPaymentSystem.Application.Commands.ProductCommands;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class UpdateProductHandler(IBaseRepository<Product> productRepository) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        request.Product.ProductName = request.ProductName;
        request.Product.Description = request.Description;
        request.Product.Cost = request.Cost;
        productRepository.Update(request.Product);
        await productRepository.SaveChangesAsync();
    }
}
