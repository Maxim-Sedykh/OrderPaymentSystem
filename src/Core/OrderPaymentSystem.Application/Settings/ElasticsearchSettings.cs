namespace OrderPaymentSystem.Application.Settings;

/// <summary>
/// Настройки ElasticSearch
/// </summary>
public class ElasticsearchSettings
{
    /// <summary>
    /// Название секции в настройках
    /// </summary>
    public const string SectionName = "ElasticConfiguration";

    /// <summary>
    /// Uri до эластика
    /// </summary>
    public string? Uri { get; set; }
}
