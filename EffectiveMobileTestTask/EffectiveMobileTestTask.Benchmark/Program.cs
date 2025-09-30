using BenchmarkDotNet.Running;

namespace EffectiveMobileTestTask.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CompanySearchBenchmark>();
        }
    }
}
