namespace OrderPaymentSystem.IntegrationTests.Constants;

/// <summary>
/// Названия образов для Docker-контейнеров
/// </summary>
public static class ContainerImages
{
    /// <summary>
    /// Образ PostgreSQL
    /// </summary>
    public const string Postgres = "postgres:latest";

    /// <summary>
    /// Образ Redis
    /// </summary>
    public const string Redis = "redis:latest";

    /// <summary>
    /// Образ Elasticsearch
    /// </summary>
    public const string Elasticsearch = "docker.elastic.co/elasticsearch/elasticsearch:8.11.0";
}
