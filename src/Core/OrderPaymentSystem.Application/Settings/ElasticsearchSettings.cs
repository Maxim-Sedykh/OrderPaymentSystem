using System;
using System.Collections.Generic;
using System.Text;

namespace OrderPaymentSystem.Application.Settings
{
    public class ElasticsearchSettings
    {
        public const string SectionName = "ElasticConfiguration";
        public string? Uri { get; set; }
    }
}
