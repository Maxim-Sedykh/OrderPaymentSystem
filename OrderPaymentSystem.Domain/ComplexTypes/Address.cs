﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OrderPaymentSystem.Domain.ComplexTypes
{
    [ComplexType]
    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
