namespace DateTimeRangeLibrary;

public class ConstructorTests
{
	[Theory, ClassData(typeof(ValidValuesData))]
	public void Constructor_WithValidValues(DateTime? begin, DateTime? end)
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
	
	private class ValidValuesData : TheoryData<DateTime?, DateTime?>
	{
		public ValidValuesData()
		{
			Add(null, null);
			Add(null, new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), null);
			Add(new DateTime(2020, 1, 2), new DateTime(2020, 1, 1));
			Add(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
		}
	}
	
	[Theory, ClassData(typeof(EdgeCasesData))]
	public void Constructor_WithEdgeCases(DateTime begin, TimeSpan duration, DateTime expectedBegin, DateTime expectedEnd)
	{
		// Act
		var range = new DateTimeRange(begin, duration);

		// Assert
		Assert.Equal(expectedBegin, range.Begin);
		Assert.Equal(expectedEnd, range.End);
	}

	private class EdgeCasesData : TheoryData<DateTime, TimeSpan, DateTime, DateTime>
	{
		public EdgeCasesData()
		{
			// защита от перехода через DateTime.MaxValue
			Set(
				begin: new DateTime(1), duration: TimeSpan.MaxValue,
				expectedBegin: new DateTime(1), expectedEnd: DateTime.MaxValue);
			
			// защита от перехода через DateTime.MinValue
			Set(
				begin: new DateTime(1), duration: new TimeSpan(-2),
				expectedBegin: DateTime.MinValue, expectedEnd: new DateTime(1));
		}
		
		private void Set(DateTime begin, TimeSpan duration, DateTime expectedBegin, DateTime expectedEnd)
			=> Add(begin, duration, expectedBegin, expectedEnd);
	}
}
