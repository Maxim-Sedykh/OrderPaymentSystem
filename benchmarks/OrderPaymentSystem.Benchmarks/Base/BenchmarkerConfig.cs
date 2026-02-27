using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;

namespace OrderPaymentSystem.Benchmarks.Base;

/// <summary>
/// Конфиг для каждого бенчмарка
/// </summary>
public class BenchmarkerConfig : ManualConfig
{
    /// <summary>
    /// Конструктор конфига, определяет, что будет выводить консоль.
    /// </summary>
    public BenchmarkerConfig()
    {
        AddLogger(ConsoleLogger.Default);

        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(MarkdownExporter.Default);
        AddColumnProvider(DefaultColumnProviders.Instance);
    }
}
