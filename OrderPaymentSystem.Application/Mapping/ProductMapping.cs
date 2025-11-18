using AutoMapper;
using OrderPaymentSystem.Domain.Dto.Product;
using OrderPaymentSystem.Domain.Entities;

namespace OrderPaymentSystem.Application.Mapping;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<Product, ProductDto>().ReverseMap();

        CreateMap<Product, CreateProductDto>().ReverseMap();
    }
}
