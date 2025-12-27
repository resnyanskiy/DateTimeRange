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

	public DateTimeRange(DateTime begin, TimeSpan duration)
	{
		//TODO Check ArgumentOutOfRangeException for crossing DateTime.Min/Max

		if (duration < TimeSpan.Zero)
		{
			Begin = begin + duration;
			End = begin;
		}
		else
		{
			Begin = begin;
			End = begin + duration;
		}
	}

	public DateTime Begin { get; }

	public DateTime End { get; }
}
