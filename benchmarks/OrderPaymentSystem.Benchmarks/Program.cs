using BenchmarkDotNet.Running;
using OrderPaymentSystem.Benchmarks.Benchmarks;

namespace OrderPaymentSystem.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<AuthServiceBenchmarks>();
        BenchmarkRunner.Run<OrderServiceBenchmarks>();
        BenchmarkRunner.Run<ProductServiceBenchmarks>();
        BenchmarkRunner.Run<UserTokenServiceBenchmarks>();
    }
}
