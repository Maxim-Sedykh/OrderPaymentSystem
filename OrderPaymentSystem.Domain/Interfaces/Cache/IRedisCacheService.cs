using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Interfaces.Cache
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string key) where T: class;

        Task<T> GetAsync<T>(string key, Func<Task<T>> factory) where T: class;

        Task SetValueAsync<T>(string key, T value) where T: class;
    }
}
