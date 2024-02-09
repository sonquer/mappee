using BenchmarkDotNet.Running;
using System;

namespace Benchmark
{
    internal class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<MappersBenchmark>();
            //BenchmarkRunner.Run<CollectionBenchmark>();

            Console.ReadKey();
        }
    }
}