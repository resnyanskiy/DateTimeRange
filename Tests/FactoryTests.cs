namespace DateTimeRangeLibrary;

public class FactoryTests
{
	[Fact]
	public void CreateFromPulse()
	{
		/*
		 * input:    + - + - + + - -
		 * output:   |-| |-| |---|
		 */

		// Arrange
		var begin = DateTime.Today;
		var pulse = new Dictionary<DateTime, bool>
		{
			[begin.AddMinutes(1)] = true,
			[begin.AddMinutes(2)] = false,
			//
			[begin.AddMinutes(3)] = true,
			[begin.AddMinutes(4)] = false,
			//
			[begin.AddMinutes(5)] = true,
			[begin.AddMinutes(6)] = true,
			[begin.AddMinutes(7)] = false,
			[begin.AddMinutes(8)] = false
		};

		// Act
		var ranges = DateTimeRange.Create(pulse, false).ToArray();

		// Assert
		Assert.Equal(3, ranges.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(1), begin.AddMinutes(2)), ranges[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(3), begin.AddMinutes(4)), ranges[1]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(5), begin.AddMinutes(7)), ranges[2]);
	}

	[Fact]
	public void CreateFromSignal_If_NoGoodSignal_Then_NoRanges()
	{
		// Arrange
		var begin = DateTime.Today;
		const int threshold = 3;
		var signal = new Dictionary<DateTime, int>()
		{
			[begin.AddMinutes(1)] = 1,
			[begin.AddMinutes(2)] = 2,
			[begin.AddMinutes(3)] = 3,
			[begin.AddMinutes(4)] = 2,
		};
		
		// Act
		var ranges = DateTimeRange.Create(signal, threshold).ToArray();
		
		// Assert
		Assert.Empty(ranges);
	}
	
	[Fact]
	public void CreateFromSignal_If_NoEndSignal_Then_LastSignalIsRangeEnd()
	{
		// Arrange
		var begin = DateTime.Today;
		const int threshold = 3;
		var signal = new Dictionary<DateTime, int>()
		{
			[begin.AddMinutes(1)] = 1,
			[begin.AddMinutes(2)] = 2,
			[begin.AddMinutes(3)] = 5,
			[begin.AddMinutes(4)] = 4,
			[begin.AddMinutes(5)] = 4,
		};
		
		// Act
		var ranges = DateTimeRange.Create(signal, threshold).ToArray();
		
		// Assert
		Assert.Single(ranges);
		Assert.Equal(begin.AddMinutes(3), ranges[0].Begin);
		Assert.Equal(begin.AddMinutes(5), ranges[0].End);		
	}
	
	[Fact]
	public void CreateFromSignal_If_OneSignal_Then_RangeWithNoEnd()
	{
		// Arrange
		var begin = DateTime.Today;
		const int threshold = 3;
		var signal = new Dictionary<DateTime, int>()
		{
			[begin.AddMinutes(1)] = 4,
			[begin.AddMinutes(2)] = 2,
			[begin.AddMinutes(3)] = 5,
		};
		
		// Act
		var ranges = DateTimeRange.Create(signal, threshold).ToArray();
		
		// Assert
		Assert.Equal(begin.AddMinutes(3), ranges[1].Begin);
		Assert.Equal(DateTime.MaxValue, ranges[1].End);		
	}
}
