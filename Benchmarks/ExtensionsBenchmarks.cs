using BenchmarkDotNet.Configs;

namespace Benchmarks;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[MemoryDiagnoser]
public class ExtensionsBenchmarks
{
	private DateTimeRange[] _ranges;
	private DateTimeRange[] _sorted;
	private DateTimeRange _random;
	
	private IEnumerable<DateTimeRange> Ranges => _ranges;

	[Params(10, 100, 1000, 10000)]
	public int N;

	/*
	[Params(0.1, 0.5, 0.9)]
	public double OverlapRatio;
	*/
	
	[GlobalSetup]
	public void Setup()
	{
		_ranges = new DateTimeRange[N];
		
		// Get random ranges
		var baseDate = new DateTime(2020, 1, 1);
		var random = new Random(42);
		for (var i = 0; i < N; i++)
		{
			var begin = baseDate.AddDays(random.NextDouble() * 365);
			var duration = TimeSpan.FromDays(365 * (0.1 + 0.9 * random.NextDouble()));
			_ranges[i] = new DateTimeRange(begin, begin + duration);
			
			/*
			// Use OverlapRatio to manage overlapping
			var end = begin + duration;
			if (i > 0 && random.NextDouble() < OverlapRatio)
			{
				var ticks = (long)((_ranges[i-1].End - _ranges[i-1].Begin).Ticks * random.NextDouble() * 0.8);
				var overlapStart = _ranges[i-1].Begin.AddTicks(ticks);
				begin = overlapStart < end ? overlapStart : begin;
			}
            
			_ranges[i] = new DateTimeRange(begin, end);
			*/
		}

		_sorted = _ranges.ToArray();
		_sorted.Sort(new DefaultComparer());
		
		_random = _ranges[random.Next(N)];
	}
	
	[BenchmarkCategory(nameof(Merge)), Benchmark(Baseline =  true)]
	public int Merge() => Ranges.Merge().Count();
	
	[BenchmarkCategory(nameof(Merge)), Benchmark]
	public int MergeSorted() => _sorted.Merge().Count();

	[BenchmarkCategory(nameof(Slice)), Benchmark(Baseline =  true)]
	public int Slice() => Ranges.Slice().Count();
	
	[BenchmarkCategory(nameof(Slice)), Benchmark]
	public int SliceSorted() => _sorted.Slice().Count();

	[BenchmarkCategory(nameof(Intersections)), Benchmark(Baseline =  true)]
	public int Intersections() => Ranges.Intersections().Count();	

	[BenchmarkCategory(nameof(Intersections)), Benchmark]
	public int IntersectionsSorted() => _sorted.Intersections().Count();	

	[BenchmarkCategory(nameof(Intersections)), Benchmark]
	public int IntersectionsWithRange() => Ranges.Intersections(_random).Count();	
}
