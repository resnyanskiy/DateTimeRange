namespace DateTimeRangeLibrary;

/// <summary>
/// Represents a range with a <see cref="Begin"/> and <see cref="End"/> points.
/// </summary>
public record struct DateTimeRange
{
	/// <summary>
	/// Creates ranges from a collection of key-value pairs. A range is created when the value crosses the specified threshold.
	/// </summary>
	/// <remarks>
	/// Useful for creating ranges from boolean signals, numeric thresholds, etc.
	/// </remarks>
	/// <typeparam name="T">The type of the value in key-value pair.</typeparam>
	/// <param name="values">Value at some point on timeline.</param>
	/// <param name="threshold">The value that determines when to create a range.</param>
	/// <returns><see cref="DateTimeRange"/> instances.</returns>	
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

	/// <summary>
	/// The minimum possible range.
	/// </summary>	
	public static readonly DateTimeRange MinValue = new(DateTime.MinValue, DateTime.MinValue);

	/// <summary>
	/// The maximum possible range.
	/// </summary>
	public static readonly DateTimeRange MaxValue = new(DateTime.MinValue, DateTime.MaxValue);
	
	/// <summary>
	/// Initializes a new instance of DateTimeRange with optional <paramref name="begin"/> and <paramref name="end"/> values.
	/// </summary>
	/// <remarks>
	/// If <c>begin</c> and <c>end</c> are provided and <c>end &lt; begin</c>, they will be swapped.
	/// </remarks>
	/// <param name="begin">The beginning of the range, or null to use <see cref="DateTime.MinValue"/>.</param>
	/// <param name="end">The end of the range, or null to use <see cref="DateTime.MaxValue"/>.</param>
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

	/// <summary>
	/// Deconstruct the range.
	/// </summary>
	/// <param name="begin">The beginning of the range.</param>
	/// <param name="duration">The duration of the range.</param>
	public void Deconstruct(out DateTime begin, out TimeSpan duration)
	{
		begin = Begin;
		duration = End - Begin;
	}

	/// <summary>
	/// The beginning of the range.
	/// </summary>	
	public DateTime Begin { get; internal init; }

	/// <summary>
	/// The end of the range.
	/// </summary>	
	public DateTime End { get; internal init; }
}
