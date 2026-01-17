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

		// Arrange
		var begin = DateTime.Today;
		var ranges = Ranges((0, 5), (6, 11), (1, 9), (2, 8));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((2, 5).Range(), intersections[0]);
		Assert.Equal((6, 8).Range(), intersections[1]);
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
		// Arrange
		var ranges = Ranges((0, 5), (9, 14), (1, 11), (2, 7));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((2, 5).Range(), intersections[0]);
		Assert.Equal((9, 11).Range(), intersections[1]);
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
	public void Overlapped_Ranges()
	{
		// Arrange
		var begin = DateTime.Today;
		var ranges = Ranges((0, 10), (1, 9), (2, 5), (6, 8));

		// Act
		var intersections = ranges.Intersections().ToArray();

		// Assert
		Assert.Equal(2, intersections.Length);
		Assert.Equal((2, 5).Range(), intersections[0]);
		Assert.Equal((6, 8).Range(), intersections[1]);
	}
}
