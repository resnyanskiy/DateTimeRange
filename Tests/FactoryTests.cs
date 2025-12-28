namespace DateTimeRangeLibrary;

public class FactoryTests
{
	[Theory, ClassData(typeof(EdgeCasesData))]
	public void Constructor_WithEdgeCases(DateTime begin, TimeSpan duration, DateTime expectedBegin, DateTime expectedEnd)
	{
		// Act
		var range = DateTimeRange.Create(begin, duration);

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
