using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;

namespace OrderPaymentSystem.Benchmarks.Base;

public class BenchmarkerConfig : ManualConfig
{
    public BenchmarkerConfig()
    {
        AddLogger(ConsoleLogger.Default);

        AddDiagnoser(MemoryDiagnoser.Default);
        AddExporter(MarkdownExporter.Default);
        AddColumnProvider(DefaultColumnProviders.Instance);
    }
}
