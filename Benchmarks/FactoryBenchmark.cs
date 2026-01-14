namespace Benchmarks;

[MemoryDiagnoser]
public class FactoryBenchmark
{
	private List<KeyValuePair<DateTime, int>> _values;
    
	[Params(100, 1000, 10000)]
	public int N;
    
	[GlobalSetup]
	public void Setup()
	{
		var begin = DateTime.Today;
		var range = Enumerable.Range(0, N);
		var values = range.Select(i => new KeyValuePair<DateTime, int>(begin.AddMinutes(i), Random.Shared.Next(0, 100)));
		_values = values.ToList();
	}
    
	[Benchmark]
	public int CreateRanges() => DateTimeRange.Create(_values, 50).Count();
}
