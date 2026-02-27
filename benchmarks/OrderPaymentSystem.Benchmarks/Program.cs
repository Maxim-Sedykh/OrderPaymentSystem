using BenchmarkDotNet.Running;
using OrderPaymentSystem.Benchmarks.Benchmarks;

namespace OrderPaymentSystem.Benchmarks;

/// <summary>
/// Программа запуска бенчмарков
/// </summary>
public class Program
{
    /// <summary>
    /// Метод запуска всех бенчмарков
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<AuthServiceBenchmarks>();
        BenchmarkRunner.Run<OrderServiceBenchmarks>();
        BenchmarkRunner.Run<ProductServiceBenchmarks>();
        BenchmarkRunner.Run<UserTokenServiceBenchmarks>();
    }
}
