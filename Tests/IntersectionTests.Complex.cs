namespace DateTimeRangeLibrary;

public partial class IntersectionTests
{
	/*
	012345678901
	/----//----/
	 /-------/
	  /-----/
	012345678901
	 /+++/
	  /+++++/
	      /+/
	012345678901
	  /++//+/
	012345678901
	*/
	[Fact]
	public void Intersections_Of_Intersections()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(6), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(8)),
			//
			new DateTimeRange(begin.AddMinutes(2), TimeSpan.FromMinutes(6)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(2), begin.AddMinutes(5)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(6), begin.AddMinutes(8)), intersections[1]);
	}
	
	/*
	012345678901234
	/----/   /----/
	 /---------/
	  /----/
	012345678901234
	 /+++/
	  /++++/
	         /+/
	012345678901
	  /++/   /+/
	012345678901
	*/
	[Fact]
	public void Connecting_Range()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(9), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(10)),
			//
			new DateTimeRange(begin.AddMinutes(2), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(2), begin.AddMinutes(5)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(9), begin.AddMinutes(11)), intersections[1]);
	}
	
	/*
	01234567890
	/---------/
	 /-------/
	  /--//-/
	01234567890
	  /++//+/
	*/
	[Fact]	
	// [Fact(Skip = "Infinite loop")]	
	public void Overlapped_Ranges()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(10)),
			//
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(8)),
			//
			new DateTimeRange(begin.AddMinutes(2), TimeSpan.FromMinutes(3)),
			new DateTimeRange(begin.AddMinutes(6), TimeSpan.FromMinutes(2)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(2), begin.AddMinutes(5)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(6), begin.AddMinutes(8)), intersections[1]);
	}
}
