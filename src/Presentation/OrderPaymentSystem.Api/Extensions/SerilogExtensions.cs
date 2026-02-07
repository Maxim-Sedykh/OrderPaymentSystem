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
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Elasticsearch([new Uri(builder.Configuration["ElasticConfiguration:Uri"]!)], opts =>
            {
                opts.DataStream = new DataStreamName("logs", "orderpaymentsystem-api", "dev");
                opts.BootstrapMethod = BootstrapMethod.Silent;
            })
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}
