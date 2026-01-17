namespace DateTimeRangeLibrary;

/// <summary>
/// Provides extension methods for enumeration of <see cref="DateTimeRange"/>.
/// </summary>
public static class Extensions
{
	/// <param name="ranges">Enumeration of ranges.</param>
	extension(IEnumerable<DateTimeRange> ranges)
	{
		/// <summary>
		/// Finds all intersections between ranges.
		/// </summary>
		/// <remarks>
		/// Returns <paramref name="ranges"/> if it has less than 2 elements.
		/// </remarks>
		/// <returns>Enumeration of ranges representing the intersections.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="ranges"/> is null.</exception>
		public IEnumerable<DateTimeRange> Intersections()
		{
			ArgumentNullException.ThrowIfNull(ranges);

			var rangesArray = ranges.ToArray();

			// if there are no ranges or only one range, return without calculations (let GC collect `rangesArray`) 
			if (rangesArray.Length < 2)
				return ranges;

			rangesArray.Sort(new DefaultComparer());
		
			return Intersections(rangesArray);
		}
		
		/// <summary>
		/// Merges overlapping ranges.
		/// </summary>
		/// <returns>Enumeration of merged ranges.</returns>	
		public IEnumerable<DateTimeRange> Merge()
		{
			ArgumentNullException.ThrowIfNull(ranges);

			using var enumerator = ranges /*.Distinct()*/.OrderBy(x => x.Begin).GetEnumerator();
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
		/// <returns>Enumeration of sliced segments.</returns>	
		public IEnumerable<DateTimeRange> Slice()
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
	}

	/// <param name="rangesArray">Array of ranges.</param>
	extension(DateTimeRange[] rangesArray)
	{
		/// <summary>
		/// Finds all intersections between ranges.
		/// </summary>
		/// <remarks>
		/// Returns empty enumeration if <paramref name="rangesArray"/> has less than 2 elements.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="rangesArray"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown in <paramref name="rangesArray"/> not sorted.</exception>
		/// <returns>Enumeration of ranges representing the intersections.</returns>
		public IEnumerable<DateTimeRange> Intersections()
		{
			ArgumentNullException.ThrowIfNull(rangesArray);

			if (rangesArray.Length < 2)
				yield break;

			var comparer = new DefaultComparer();

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
					if (comparer.Compare(rangesArray[currentIndex-1], rangesArray[currentIndex]) > 0)
						throw new ArgumentException("Ranges are not sorted", nameof(rangesArray));
				
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

	private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

	private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}
