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
			Add(null, new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), null);
			Add(new DateTime(2020, 1, 2), new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
		}
	}
	
	[Fact]
	public void DefaultConstructor_CreatesMinValue()
	{
		// Act
		var range = new DateTimeRange();
		
		// Assert
		Assert.Equal(DateTimeRange.MinValue, range);
	}
	
	[Fact]
	public void Constructor_WithNulls_CreatesMaxValue()
	{
		// Act
		var range = new DateTimeRange(null, null);
		
		// Assert
		Assert.Equal(DateTimeRange.MaxValue, range);
	}	
}
