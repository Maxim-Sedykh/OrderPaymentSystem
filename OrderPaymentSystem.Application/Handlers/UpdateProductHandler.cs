using MediatR;
using OrderPaymentSystem.Application.Commands;
using OrderPaymentSystem.Domain.Entity;
using OrderPaymentSystem.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Handlers
{
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
}
