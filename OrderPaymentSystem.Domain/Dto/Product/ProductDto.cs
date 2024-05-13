using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPaymentSystem.Domain.Dto.Product
{
    //public record ProductDto(long Id, string ProductName, string Description, decimal Cost, string CreatedAt);

    public class ProductDto
    {
        public long Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }

        public string CreatedAt { get; set;}
    }
}
