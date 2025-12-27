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
		if (duration < TimeSpan.Zero)
		{
			End = begin;

			try
			{
				Begin = begin + duration;
			}
			catch (ArgumentOutOfRangeException)
			{
				Begin = DateTime.MinValue;
			}
		}
		else
		{
			Begin = begin;
			
			try
			{
				End = begin + duration;
			}
			catch (ArgumentOutOfRangeException)
			{
				End = DateTime.MaxValue;
			}
		}
	}

	public DateTime Begin { get; }

	public DateTime End { get; }
}
