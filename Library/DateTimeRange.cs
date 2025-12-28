namespace DateTimeRangeLibrary;

public record struct DateTimeRange
{
	public DateTimeRange(DateTime? begin, DateTime? end)
	{
		if (begin != null && end != null && end < begin)
		{
			Begin = (DateTime)end;
			End = (DateTime)begin;
		}
		else
		{
			Begin = begin ?? DateTime.MinValue;
			End = end ?? DateTime.MaxValue;
		}
	}

	// could throw ArgumentOutOfRangeException if `begin + duration` is out of `DateTime.MinValue..DateTime.MaxValue`
	public DateTimeRange(DateTime begin, TimeSpan duration) : this(begin, begin + duration)
	{
		//
	}

	public DateTime Begin { get; }

	public DateTime End { get; }
}
