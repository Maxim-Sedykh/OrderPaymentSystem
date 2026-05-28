namespace OrderPaymentSystem.Application.Settings;

/// <summary>
/// Настройки resilience-пайплайнов для Cache, Database и HttpClient.
/// </summary>
public sealed class ResilienceSettings
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    public const string SectionName = "Resilience";

    /// <summary>
    /// Resilience для Redis-кэша. Retry + Timeout, без Circuit Breaker.
    /// </summary>
    public PipelineOptions Cache { get; set; } = new()
    {
        RetryCount = 2,
        RetryDelaySeconds = 0.5,
        TimeoutSeconds = 5.0,
        FailureRatio = 0,
        MinimumThroughput = 0,
        SamplingDurationSeconds = 0,
        BreakDurationSeconds = 0
    };

    /// <summary>
    /// Resilience для БД (EF Core). Retry + Circuit Breaker.
    /// </summary>
    public PipelineOptions Database { get; set; } = new();

    /// <summary>
    /// Resilience для HTTP-клиентов. Retry + Circuit Breaker + Timeout.
    /// </summary>
    public PipelineOptions HttpClient { get; set; } = new();
}
