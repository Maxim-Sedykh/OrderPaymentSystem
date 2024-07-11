using MediatR;
using OrderPaymentSystem.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Application.Commands
{
    public record DeleteProductCommand(Product Product) : IRequest;
}
