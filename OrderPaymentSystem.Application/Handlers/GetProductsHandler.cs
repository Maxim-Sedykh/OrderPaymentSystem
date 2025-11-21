using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Domain.Dto.Order;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class GetProductsHandler(IBaseRepository<Product> productRepository, IMapper mapper) : IRequestHandler<GetProductsQuery, ProductDto[]>
{
    public async Task<ProductDto[]> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetQueryable()
            .AsNoTracking()
            .AsProjected<Product, ProductDto>(mapper)
            .ToArrayAsync(cancellationToken);
    }
}
