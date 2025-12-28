namespace DateTimeRangeLibrary;

public class ConstructorTests
{
	[Theory, ClassData(typeof(ConstructorValues))]
	public void Constructor_AcceptsAnyValues(DateTime? begin, DateTime? end)
	{
		// Act
		var range = new DateTimeRange(begin, end);

		// Assert
		var (expectedBegin, expectedEnd) = (begin, end) switch
		{
			(null, null) => (DateTime.MinValue, DateTime.MaxValue),
			(null, not null) => (DateTime.MinValue, end.Value),
			(not null, null) => (begin.Value, DateTime.MaxValue),
			(not null, not null) when end < begin => (end.Value, begin.Value),
			_  => (begin.Value, end.Value),
		};
		
		Assert.Equal(expectedBegin, range.Begin);
		Assert.Equal(expectedEnd, range.End);
	}
	
	private class ConstructorValues : TheoryData<DateTime?, DateTime?>
	{
		public ConstructorValues()
		{
			Add(null, null);
			Add(null, new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), null);
			Add(new DateTime(2020, 1, 2), new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
		}
	}

	[Theory, ClassData(typeof(EdgeCasesData))]
	public void Constructor_WithEdgeCases(DateTime begin, TimeSpan duration)
	{
		// Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => new DateTimeRange(begin, duration));
	}

	private class EdgeCasesData : TheoryData<DateTime, TimeSpan>
	{
		public EdgeCasesData()
		{
			Add(new DateTime(1), TimeSpan.MaxValue);
			Add(new DateTime(1), new TimeSpan(-2));
		}
	}
	
	[Fact]
	public void DefaultConstructor_CreatesZeroRange()
	{
		// Act
		var range = new DateTimeRange();
		
		// Assert
		Assert.Equal(DateTime.MinValue, range.Begin);
		Assert.Equal(DateTime.MinValue, range.End);
	}
}
