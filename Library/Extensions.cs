namespace DateTimeRangeLibrary;

/// <summary>
/// Provides extension methods for enumeration of <see cref="DateTimeRange"/>.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Finds all intersections between ranges.
	/// </summary>
	/// <param name="ranges">Enumeration of ranges to find intersections for.</param>
	/// <returns>Enumeration of ranges representing the intersections.</returns>
	public static IEnumerable<DateTimeRange> Intersections(this IEnumerable<DateTimeRange> ranges)
	{
		ArgumentNullException.ThrowIfNull(ranges);

		var arr = ranges.ToArray();

		// if there are no ranges or only one range, return without calculations (let GC collect `arr`) 
		if (arr.Length < 2)
			return ranges;

		return GetEnumerable(arr);//.Distinct();

		IEnumerable<DateTimeRange> GetEnumerable(DateTimeRange[] rangesArray)
		{
			rangesArray.Sort(new DefaultComparer());

			var lastIntersection = new DateTimeRange(); // Begin = End = DateTime.MinValue;
			for (var baseIndex = 0; baseIndex < rangesArray.Length - 1; baseIndex++)
			{
				var baseRange = rangesArray[baseIndex];
				if (baseRange.End <= lastIntersection.End)
					continue;
				
				var maxBegin = baseRange.Begin;
				var minEnd = baseRange.End;
				var hasIntersection = false;
				
				// at this point the `rangesArray` contains at least 2 elements
				for (var currentIndex = baseIndex + 1; currentIndex < rangesArray.Length; currentIndex++)
				{
					var currentRange = rangesArray[currentIndex];

					// no intersection, go to next `base range`
					if (baseRange.End < currentRange.Begin)
						break;

					// skip `current range` if it should not be used 
					if (currentRange.End < maxBegin)
						continue;

					// start new `intersections segment`
					if (minEnd < currentRange.Begin)
					{
						if (lastIntersection.End < maxBegin)
						{
							yield return (lastIntersection = new DateTimeRange { Begin = maxBegin, End = minEnd });
						}

						currentIndex = baseIndex;
						maxBegin = currentRange.Begin;
						minEnd = baseRange.End;
						continue; //currentIndex will be baseIndex + 1
					}

					hasIntersection = true;
					maxBegin = Max(maxBegin, currentRange.Begin);
					minEnd = Min(minEnd, currentRange.End);
				}

				if (!hasIntersection || maxBegin <= lastIntersection.End) 
					continue;
				
				yield return (lastIntersection = new DateTimeRange { Begin = maxBegin, End = minEnd });
			}
		}
	}

	/// <summary>
	/// Merges overlapping ranges.
	/// </summary>
	/// <param name="ranges">Enumeration of ranges to merge.</param>
	/// <returns>Enumeration of merged ranges.</returns>	
	public static IEnumerable<DateTimeRange> Merge(this IEnumerable<DateTimeRange> ranges)
	{
		ArgumentNullException.ThrowIfNull(ranges);
		
		using var enumerator = ranges/*.Distinct()*/.OrderBy(x => x.Begin).GetEnumerator();
		var available = enumerator.MoveNext();
		while (available)
		{
			var begin = enumerator.Current.Begin;
			var end = enumerator.Current.End;

			while ((available = enumerator.MoveNext()) && enumerator.Current.Begin <= end)
			{
				if (end < enumerator.Current.End)
				{
					end = enumerator.Current.End;
				}
			}

			yield return new DateTimeRange(begin, end);
		}
	}

	/// <summary>
	/// Slices provided ranges into non-overlapping segments based on their boundary points.
	/// </summary>
	/// <param name="ranges">Enumeration of ranges.</param>
	/// <returns>Enumeration of sliced segments.</returns>	
	public static IEnumerable<DateTimeRange> Slice(this IEnumerable<DateTimeRange> ranges)
	{
		ArgumentNullException.ThrowIfNull(ranges);

		var dates = new SortedSet<DateTime>();

		foreach (var range in ranges)
		{
			dates.Add(range.Begin);
			dates.Add(range.End);
		}

		using var enumerator = dates.GetEnumerator();
		var available = enumerator.MoveNext();
		while (available)
		{
			var begin = enumerator.Current;
			if (available = enumerator.MoveNext())
			{
				yield return new DateTimeRange(begin, enumerator.Current);
			}
		}
	}
	
	private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

	private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
