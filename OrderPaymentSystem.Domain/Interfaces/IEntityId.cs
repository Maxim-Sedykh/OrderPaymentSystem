using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces
{
    public interface IEntityId<T> where T : struct
    {
        public T Id { get; set; }
    }
}
