using BenchmarkDotNet.Running;

namespace Benchmark
{
    internal class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<PrimitiveTypeBenchmark>();
            //BenchmarkRunner.Run<CollectionBenchmark>();

            Console.ReadKey();
        }
    }
}