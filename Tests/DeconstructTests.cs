namespace DateTimeRangeLibrary;

public class DeconstructTests
{
	[Fact]
	public void Deconstruct_To_BeginAndDuration()
	{
		// Arrange
		var today = DateTime.Today;
		var range = new DateTimeRange(today, today.AddDays(1));

		// Act
		var (begin, span) = range;
		
		// Assert
		Assert.Equal(today, begin);
		Assert.Equal(TimeSpan.FromDays(1), span);
	}
}
