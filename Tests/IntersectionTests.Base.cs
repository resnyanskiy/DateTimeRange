namespace DateTimeRangeLibrary;

public partial class IntersectionTests
{
	/*
	01234567
	/----/
	 /----/
	  /----/
	01234567
	  /++/
	*/
	[Fact]
	public void BaseSample()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(2), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(2), begin.AddMinutes(5)), intersections[0]);
	}

	/*
	0123456789
	/-----/
	 /-----/
	  /-----/
	   /-----/
	0123456789
	 /++++/
	  /++++/
	   /++++/
	0123456789
	   /++/
	0123456789
	*/
	[Fact]
	public void Intersection_Of_Intersections()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(6)),
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(6)),
			new DateTimeRange(begin.AddMinutes(2), TimeSpan.FromMinutes(6)),
			new DateTimeRange(begin.AddMinutes(3), TimeSpan.FromMinutes(6)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(3), begin.AddMinutes(6)), intersections[0]);
	}

	/*
	/----/
	/----/
	/----/
	/++++/
	*/
	[Fact]
	public void If_SameRanges_Then_SingleIntersection()
	{
		var begin = DateTime.Today;
		var range = new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5));

		// Arrange
		var ranges = new[] { range, range, range };

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(0), begin.AddMinutes(5)), intersections[0]);
	}

	/*
	0123456789
	/----/
	/--------/
	/------/
	0123456789
	/++++/
	*/
	[Fact]
	public void Intersection_For_SameBegin()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(9)),
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(7)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(0), begin.AddMinutes(5)), intersections[0]);
	}

	/*
	0123456789
	    /----/
	/--------/
	  /------/
	0123456789
	    /++++/
	*/
	[Fact]
	public void Intersection_For_SameEnd()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(9), TimeSpan.FromMinutes(-5)),
			new DateTimeRange(begin.AddMinutes(9), TimeSpan.FromMinutes(-9)),
			new DateTimeRange(begin.AddMinutes(9), TimeSpan.FromMinutes(-7)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(4), begin.AddMinutes(9)), intersections[0]);
	}

	/*
	012345678901
	/----//----/
	   /----/
	012345678901
	   /+//+/
	*/
	[Fact]
	public void Two_Intersections()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(6), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(3), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(3), begin.AddMinutes(5)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(6), begin.AddMinutes(8)), intersections[1]);
	}
	
	/*
	0123456789012345
	/----/    /----/
	     /----/
	0123456789012345
	     +    +
	*/
	[Fact]
	public void BeginAndEnd_Are_PartsOfTheRange()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(10), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(5), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(5), TimeSpan.Zero), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(10), TimeSpan.Zero), intersections[1]);
	}

	/*
	012345678901234567
	/----/      /----/
	      /----/
	012345678901234567
	*/
	[Fact]
	public void NoIntersection()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(12), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(6), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections();

		// Assert
		Assert.Empty(intersections);
	}

	/*
	01234567890123
	/----/ /----/
	 /----/ /----/
	01234567890123
	 /+++/  /+++/
	*/
	[Fact]
	public void Distinct_Intersections()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(7), TimeSpan.FromMinutes(5)),
			//
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(5)),
			new DateTimeRange(begin.AddMinutes(8), TimeSpan.FromMinutes(5)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(4)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(8), TimeSpan.FromMinutes(4)), intersections[1]);
	}

	/*
	0123456789
	/--------/
	 /--//--/
	0123456789
	 /++//++/
	*/
	[Fact]
	public void Overlapped_Range()
	{
		var begin = DateTime.Today;

		// Arrange
		var ranges = new[]
		{
			new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(9)),
			//
			new DateTimeRange(begin.AddMinutes(1), TimeSpan.FromMinutes(3)),
			new DateTimeRange(begin.AddMinutes(5), TimeSpan.FromMinutes(3)),
		};

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(1), begin.AddMinutes(4)), intersections[0]);
		Assert.Equal(new DateTimeRange(begin.AddMinutes(5), begin.AddMinutes(8)), intersections[1]);
	}

	/*
	/----/
	/++++/
	*/
	[Fact]
	public void NoCalculation_If_SingleRange()
	{
		var begin = DateTime.Today;
		var range = new DateTimeRange(begin.AddMinutes(0), TimeSpan.FromMinutes(5));

		// Arrange
		var ranges = new[] { range };

		// Act
		var intersections = ranges.Intersections();

		// Assert
		Assert.Same(ranges, intersections);
	}

	[Fact]
	public void NoCalculation_If_EmptyInput()
	{
		// Arrange
		var ranges = Enumerable.Empty<DateTimeRange>();

		// Act
		var intersections = ranges.Intersections();

		// Assert
		Assert.Same(ranges, intersections);
	}
}
