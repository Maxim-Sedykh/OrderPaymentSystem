namespace OrderPaymentSystem.Application.Settings;

/// <summary>
/// Настройки resilience-пайплайна (Retry, Circuit Breaker, Timeout).
/// Применяются к кэшу, БД и HTTP-клиентам через <see cref="ResilienceSettings"/>.
/// </summary>
public sealed class PipelineOptions
{
    /// <summary>
    /// Количество повторных попыток при transient-ошибке.
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Базовая задержка между попытками в секундах (exponential backoff).
    /// </summary>
    public double RetryDelaySeconds { get; set; } = 1.0;

    /// <summary>
    /// Доля неудачных запросов для срабатывания Circuit Breaker (0.0–1.0).
    /// </summary>
    public double FailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Минимальное количество запросов в окне замера для Circuit Breaker.
    /// </summary>
    public int MinimumThroughput { get; set; } = 10;

    /// <summary>
    /// Длительность окна замера в секундах для Circuit Breaker.
    /// </summary>
    public double SamplingDurationSeconds { get; set; } = 30.0;

    /// <summary>
    /// Длительность разрыва цепи в секундах (Circuit Breaker).
    /// </summary>
    public double BreakDurationSeconds { get; set; } = 30.0;

    /// <summary>
    /// Общий таймаут операции в секундах.
    /// </summary>
    public double TimeoutSeconds { get; set; } = 30.0;
}
