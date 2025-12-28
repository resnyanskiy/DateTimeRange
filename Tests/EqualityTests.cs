namespace DateTimeRangeLibrary;

public class EqualityTests
{
	private static readonly DateTimeRange Range;
	private static readonly DateTimeRange Other;
	private static readonly DateTimeRange Equal;

	static EqualityTests()
	{
		Range = new DateTimeRange(DateTime.Now, TimeSpan.FromHours(-1));
		Other = new DateTimeRange(Range.Begin, TimeSpan.FromMinutes(1));
		Equal = new DateTimeRange(Range.End, Range.Begin);
	}

	[Fact]
	public void Equals_Assert()
	{
		Assert.Equal(Range, Equal);
		Assert.NotEqual(Range, Other);
	}

	[Fact]
	public void Operators_Assert()
	{
		Assert.True(Range == Equal);
		Assert.True(Range != Other);
	}

	[Fact]
	public void GetHashCode_Assert()
	{
		var set = new HashSet<DateTimeRange>();
		Assert.True(set.Add(Range));
		Assert.False(set.Add(Equal));
		Assert.True(set.Add(Other));
	}
}
