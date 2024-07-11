﻿using MediatR;
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
    public class DeleteProductHandler(IBaseRepository<Product> productRepository) : IRequestHandler<DeleteProductCommand>
    {
        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            productRepository.Remove(request.Product);
            await productRepository.SaveChangesAsync();
        }
    }
}
