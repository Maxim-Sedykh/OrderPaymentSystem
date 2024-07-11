using MediatR;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Commands
{
    public record UpdateProductCommand(string ProductName, string Description, decimal Cost, Product Product) : IRequest;
}
