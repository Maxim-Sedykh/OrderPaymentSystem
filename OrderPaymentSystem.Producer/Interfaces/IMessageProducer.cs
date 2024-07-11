using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Producer.Interfaces
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message, string routingKey, string? exchange = default);
    }
}
