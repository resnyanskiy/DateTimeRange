namespace DateTimeRangeLibrary;

public partial class IntersectionTests
{
	private static IEnumerable<DateTimeRange> Ranges(params (int start, int end)[] ranges) => ranges.ToRanges();
	
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
		// Arrange
		var ranges = Ranges((0, 5), (1, 6), (2, 7));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal((2, 5).Range(), intersections[0]);
	}

	/*
	01234567
	 /----/
	/----/
	  /----/
	01234567
	*/
	[Fact]
	public void InputArray_MustBe_Sorted()
	{
		// Arrange
		DateTimeRange[] ranges = Ranges((1, 6), (0, 5), (2, 7)).ToArray();

		// Act
		var intersections = ranges.Intersections();
		
		// Assert
		Assert.Throws<ArgumentException>(() => intersections.Count());
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
		// Arrange
		var ranges = Ranges((0, 6), (1, 7), (2, 8), (3, 9));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal((3, 6).Range(), intersections[0]);
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
		// Arrange
		var range = (0, 5).Range();
		var ranges = new[] { range, range, range };

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal((0, 5).Range(), intersections[0]);
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
		// Arrange
		var ranges = Ranges((0, 5), (0, 9), (0, 7));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal((0, 5).Range(), intersections[0]);
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
		// Arrange
		var ranges = Ranges((4, 9), (0, 9), (2, 9));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Single(intersections);
		Assert.Equal((4, 9).Range(), intersections[0]);
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
		// Arrange
		var ranges = Ranges((0, 5), (6, 11), (3, 8));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((3, 5).Range(), intersections[0]);
		Assert.Equal((6, 8).Range(), intersections[1]);
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
		// Arrange
		var begin = DateTime.Today;
		var ranges = Ranges((0, 5), (10, 15), (5, 10));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((5, 5).Range(), intersections[0]);
		Assert.Equal((10, 10).Range(), intersections[1]);
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
		// Arrange
		var ranges = Ranges((0, 5), (12, 17), (6, 11));

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
		// Arrange
		var ranges = Ranges((0, 5), (7, 12), (1, 6), (8, 12));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((1, 5).Range(), intersections[0]);
		Assert.Equal((8, 12).Range(), intersections[1]);
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
		// Arrange
		var ranges = Ranges((0, 9), (1, 4), (5, 8));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((1, 4).Range(), intersections[0]);
		Assert.Equal((5, 8).Range(), intersections[1]);
	}

	/*
	/----/
	/++++/
	*/
	[Fact]
	public void NoCalculation_If_SingleRange()
	{
		// Arrange
		IEnumerable<DateTimeRange> ranges = Ranges((0, 5));
		DateTimeRange[] rangesArray = [(0, 5).Range()];

		// Act
		var intersections = ranges.Intersections();
		var intersectionsOfArray = rangesArray.Intersections();

		// Assert
		Assert.Same(ranges, intersections);
		Assert.NotSame(rangesArray, intersectionsOfArray);
		Assert.Empty(intersectionsOfArray);
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
