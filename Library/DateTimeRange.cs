namespace DateTimeRangeLibrary;

public record struct DateTimeRange
{
	public static IEnumerable<DateTimeRange> Create<T>(IEnumerable<KeyValuePair<DateTime, T>> values, T threshold)
		where T : IComparable<T>
	{
		using var enumerator = values.GetEnumerator();

		while (enumerator.MoveNext())
		{
			// start interval if value is greater than threshold
			if (enumerator.Current.Value.CompareTo(threshold) > 0)
			{
				var begin = enumerator.Current.Key;
				var end = DateTime.MaxValue;
				var available = false;

				do
				{
					if (available = enumerator.MoveNext())
					{
						end = enumerator.Current.Key;
						if (end < begin)
							throw new ArgumentException("Enumeration must be sorted by key.", nameof(values));
					}
				}
				// extend interval if value is greater than threshold
				while (available && enumerator.Current.Value.CompareTo(threshold) > 0);

				yield return new DateTimeRange { Begin = begin, End = end };
			}
		}
	}
	
	public static readonly DateTimeRange MinValue = new(DateTime.MinValue, DateTime.MinValue);
	
	public static readonly DateTimeRange MaxValue = new(DateTime.MinValue, DateTime.MaxValue);
	
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

	public DateTime Begin { get; internal init; }

	public DateTime End { get; internal init; }
}
