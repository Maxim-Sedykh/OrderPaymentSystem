using AutoMapper;
using MediatR;
using OrderPaymentSystem.Application.Commands;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class CreateProductHandler(IBaseRepository<Product> productRepository, IMapper mapper) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product()
        {
            ProductName = request.ProductName,
            Description = request.Description,
            Cost = request.Cost,
        };
        await productRepository.CreateAsync(product);
        await productRepository.SaveChangesAsync();

        return mapper.Map<ProductDto>(product);
    }
}
