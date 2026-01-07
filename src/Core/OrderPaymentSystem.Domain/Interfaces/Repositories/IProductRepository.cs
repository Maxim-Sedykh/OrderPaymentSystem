using OrderPaymentSystem.Domain.Entities;
using OrderPaymentSystem.Domain.Interfaces.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Interfaces.Repositories;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product> GetById(int id, CancellationToken cancellationToken = default);
    Task<Product> GetById(int id, CancellationToken cancellationToken = default);
}
