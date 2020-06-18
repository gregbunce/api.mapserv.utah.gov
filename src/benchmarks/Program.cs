using System;
using BenchmarkDotNet.Running;
using benchmarks.tests;

namespace benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args is null) {
                throw new ArgumentNullException(nameof(args));
            }

            _ = BenchmarkRunner.Run<AddressParsingBenchMark>();
        }
    }
}
