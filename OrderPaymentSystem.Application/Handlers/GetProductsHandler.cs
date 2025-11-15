using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class GetProductsHandler(IBaseRepository<Product> productRepository) : IRequestHandler<GetProductsQuery, ProductDto[]>
{
    public async Task<ProductDto[]> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetAll()
            .AsNoTracking()
            .Select(x => new ProductDto()
            {
                Id = x.Id,
                ProductName = x.ProductName,
                Description = x.Description,
                Cost = x.Cost,
                CreatedAt = x.CreatedAt.ToLongDateString()
            })
            .ToArrayAsync(cancellationToken);
    }
}
