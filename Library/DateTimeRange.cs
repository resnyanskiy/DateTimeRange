namespace DateTimeRangeLibrary;

public record struct DateTimeRange
{
	public static IEnumerable<DateTimeRange> Create<T>(IEnumerable<KeyValuePair<DateTime, T>> values, T threshold)
		where T : IComparable<T>
	{
		using var enumerator = values.GetEnumerator();

		while (enumerator.MoveNext())
		{
			// начать интервал, если значение больше порога
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
							throw new ArgumentException("Dictionary must be sorted by key.", nameof(values));
					}
				}
				// продлевать интервал, если значение больше порога
				while (available && enumerator.Current.Value.CompareTo(threshold) > 0);

				yield return new DateTimeRange(begin, end);
			}
		}
	}
	
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
