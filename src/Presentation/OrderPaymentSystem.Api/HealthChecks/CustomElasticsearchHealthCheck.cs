using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Settings;

namespace OrderPaymentSystem.Api.HealthChecks;

/// <summary>
/// Кастомный HealthCheck для ElasticSearch.
/// Написал свой, так как использование HealthCheck.ElasticSearch у меня вызывает ошибку 
/// с транзитивностью этого пакета к пакету ElasticSearch.Channels.
/// </summary>
public class CustomElasticsearchHealthCheck : IHealthCheck
{
    /// <summary>
    /// Uri до эластика
    /// </summary>
    private readonly Uri? _elasticUri;

    /// <summary>
    /// Фабрика для создания Http клиента
    /// </summary>
    private readonly IHttpClientFactory? _httpClientFactory;

    /// <summary>
    /// Конструктор кастомного healthcheck'а
    /// </summary>
    /// <param name="elasticOptions">Конфиги эластика</param>
    /// <param name="httpClientFactory">Фабрика создания http клиентов</param>
    public CustomElasticsearchHealthCheck(IOptions<ElasticsearchSettings> elasticOptions, IHttpClientFactory httpClientFactory)
    {
        var elasticUrlString = elasticOptions.Value.Uri;

        if (string.IsNullOrEmpty(elasticUrlString))
        {
            _elasticUri = null;
            return;
        }

        if (elasticUrlString.Contains("localhost", StringComparison.OrdinalIgnoreCase))
        {
            _elasticUri = new Uri(elasticUrlString.Replace("localhost", "127.0.0.1", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            _elasticUri = new Uri(elasticUrlString);
        }

        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Проверить работоспособность ElasticSearch
    /// </summary>
    /// <param name="context">Контекст проверки работоспособности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns><see cref="HealthCheckResult"/></returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_elasticUri == null)
        {
            return HealthCheckResult.Unhealthy("Elasticsearch URL is not configured or invalid.");
        }

        try
        {
            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var httpClient = _httpClientFactory!.CreateClient();

            httpClient.Timeout = TimeSpan.FromSeconds(10);
            httpClient.BaseAddress = _elasticUri;

            var response = await httpClient.GetAsync("/_cluster/health", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            if (content.Contains("\"status\":\"green\"") || content.Contains("\"status\":\"yellow\""))
            {
                return HealthCheckResult.Healthy("Elasticsearch is healthy.");
            }
            else
            {
                return HealthCheckResult.Degraded($"Elasticsearch status is not green/yellow: {content}");
            }
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckResult.Unhealthy($"Elasticsearch is inaccessible at {_elasticUri}. Make sure it's running and reachable: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Elasticsearch health check failed due to an unexpected error at {_elasticUri}: {ex.Message}", ex);
        }
    }
}
