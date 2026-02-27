using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Serilog;

namespace OrderPaymentSystem.Api.Extensions;

/// <summary>
/// Настроить логгирование
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Зарегистрировать логгирование в ElasticSearch
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            var elasticUri = context.Configuration["ElasticConfiguration:Uri"];

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .ReadFrom.Configuration(context.Configuration);

            if (!string.IsNullOrEmpty(elasticUri))
            {
                configuration.WriteTo.Elasticsearch([new Uri(elasticUri)], opts =>
                {
                    opts.DataStream = new DataStreamName("logs", "orderpaymentsystem-api", "dev");
                    opts.BootstrapMethod = BootstrapMethod.Silent;
                });
            }
        });
    }
}
