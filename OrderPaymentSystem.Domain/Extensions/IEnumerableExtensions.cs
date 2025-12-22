using OrderPaymentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Domain.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }
}
