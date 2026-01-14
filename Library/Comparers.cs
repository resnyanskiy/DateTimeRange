namespace DateTimeRangeLibrary;

/*
 * Sorting time intervals is ambiguous - which interval is _larger_, the one that is _later_, or the one that is _longer_?
 * Therefore, the comparison should be performed based on an "external strategy" - a Comparer.
 */

public class DefaultComparer : IComparer<DateTimeRange>
{
	public int Compare(DateTimeRange x, DateTimeRange y)
	{
		var compareBegin = x.Begin.CompareTo(y.Begin);
		return compareBegin != 0 ? compareBegin : x.End.CompareTo(y.End);
	}
}

public class AlternateComparer : IComparer<DateTimeRange>
{
	public int Compare(DateTimeRange x, DateTimeRange y)
	{
		var compareEnd = x.End.CompareTo(y.End);
		return compareEnd != 0 ? compareEnd : x.Begin.CompareTo(y.Begin);
	}
}
