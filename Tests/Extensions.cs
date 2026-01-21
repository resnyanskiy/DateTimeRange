namespace DateTimeRangeLibrary;

internal static class Extensions
{
	public static DateTimeRange Range(this (int begin, int end) range)
	{
		var today = DateTime.Today;
		return new DateTimeRange(today.AddMinutes(range.begin), today.AddMinutes(range.end));
	}
	
	public static IEnumerable<DateTimeRange> Ranges(this (int begin, int end)[] ranges)
	{
		var today = DateTime.Today;
		foreach (var range in ranges)
		{
			yield return new DateTimeRange(today.AddMinutes(range.begin), today.AddMinutes(range.end));
		}
	}
}
