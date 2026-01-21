namespace DateTimeRangeLibrary;

public class IntervalTreeTests
{
	private static IEnumerable<DateTimeRange> Ranges(params (int start, int end)[] ranges) => ranges.Ranges();

	[Fact]
	public void SearchIntersections_EmptyTree_EmptyIntersection()
	{
		// Arrange
		var tree = new IntervalTree([]);
		var range = (0, 1).Range();

		// Act
		var result = tree.SearchIntersections(range);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void SearchIntersections_SingleRange_NoIntersection()
	{
		// Arrange
		var ranges = Ranges((1, 5));
		var tree = new IntervalTree(ranges);
		var range = (6, 10).Range();

		// Act
		var result = tree.SearchIntersections(range);

		// Assert
		Assert.Empty(result);
	}

	[Fact]
	public void SearchIntersections_SingleRange_Intersection()
	{
		// Arrange
		var ranges = Ranges((1, 10)).ToArray();
		var tree = new IntervalTree(ranges);
		var range = (5, 15).Range();

		// Act
		var result = tree.SearchIntersections(range).ToArray();

		// Assert
		Assert.Single(result);
		Assert.Equal(ranges[0], result[0]);
	}
	
	[Fact]
	public void SearchIntersections_RangeInsideRanges_Intersection()
	{
		// Arrange
		var ranges = Ranges((1, 10)).ToArray();
		var tree = new IntervalTree(ranges);
		var range = (5, 7).Range();

		// Act
		var result = tree.SearchIntersections(range).ToList();

		// Assert
		Assert.Single(result);
		Assert.Equal(ranges[0], result[0]);
	}	

	[Fact]
	public void SearchIntersections_RangesInsideRange_Intersection()
	{
		// Arrange
		var ranges = Ranges((5, 7));
		var tree = new IntervalTree(ranges);
		var range = (1, 10).Range();

		// Act
		var result = tree.SearchIntersections(range);

		// Assert
		Assert.Equal(ranges, result);
	}

	[Fact]
	public void SearchIntersections_OverlappingRanges_ReturnsAll()
	{
		// Arrange
		var ranges = Ranges((1, 10), (5, 15), (12, 20));
		var tree = new IntervalTree(ranges);
		var range = (8, 18).Range();

		// Act
		var result = tree.SearchIntersections(range).OrderBy(r => r.Begin);

		// Assert
		Assert.Equal(ranges, result);
	}

	[Fact]
	public void SearchIntersections_TouchesBoundaries()
	{
		// Arrange
		var ranges = Ranges((1, 10), (15, 25));
		var tree = new IntervalTree(ranges);
		var range = (10, 15).Range();

		// Act
		var result = tree.SearchIntersections(range);

		// Assert
		Assert.Equal(ranges, result);
	}

	[Fact]
	public void SearchIntersections_vs_NaiveSearch()
	{
		// Arrange
		var random = new Random(42);
		var ranges = Enumerable.Range(0, 1000).Select(i =>
		{
			var begin = new DateTime(2024, 1, 1).AddDays(random.Next(0, 300));
			var end = begin.AddDays(random.Next(1, 30));
			return new DateTimeRange(begin, end);
		})
		.ToList();

		var tree = new IntervalTree(ranges);
		var range = new DateTimeRange(new DateTime(2024, 2, 1), new DateTime(2024, 6, 1));

		// Act
		var result = tree.SearchIntersections(range).ToList();
		var bruteForceResult = ranges.Where(r => r.Begin <= range.End && range.Begin <= r.End).ToList();

		// Assert
		Assert.Equal(bruteForceResult.Count, result.Count);
		Assert.All(result, r => Assert.True(r.Begin <= range.End && range.Begin <= r.End));
	}
}
