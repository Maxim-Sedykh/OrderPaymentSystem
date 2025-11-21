using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderPaymentSystem.Application.Queries;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Extensions;
using OrderPaymentSystem.Domain.Interfaces.Repositories;

namespace OrderPaymentSystem.Application.Handlers;

public class GetProductByIdHandler(IBaseRepository<Product> productRepository, IMapper mapper) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await productRepository.GetQueryable()
                    .Where(x => x.Id == request.ProductId)
                    .AsProjected<Product, ProductDto>(mapper)
                    .FirstOrDefaultAsync(cancellationToken);
    }
}
