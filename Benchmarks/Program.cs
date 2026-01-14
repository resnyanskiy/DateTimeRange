using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Benchmarks;

public class Program
{
	public static void Main(string[] args)
	{
		var config = DefaultConfig.Instance.WithOptions(ConfigOptions.DisableLogFile);
		BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
	}
}
