using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OrderPaymentSystem.Application.Settings;

namespace OrderPaymentSystem.Api.HealthChecks
{

    public class CustomElasticsearchHealthCheck : IHealthCheck
    {
        private readonly Uri? _elasticUri;
        private readonly IHttpClientFactory _httpClientFactory;

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

                using var httpClient = _httpClientFactory.CreateClient();

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
}
