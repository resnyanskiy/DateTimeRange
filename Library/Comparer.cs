namespace DateTimeRangeLibrary;

/*
 * Сортировка отрезков времени неоднозначна - какой отрезок _больше_, тот, что _позже_, или тот, что _дольше_?
 * Поэтому сравнение должно выполняться на основе "внешней стратегии" - Comparer'a.
 */

public class Comparer : IComparer<DateTimeRange>
{
	public int Compare(DateTimeRange x, DateTimeRange y)
	{
		// Сравнение по окончанию только если начала равны
		var compareBegin = x.Begin.CompareTo(y.Begin);
		return compareBegin != 0 ? compareBegin : x.End.CompareTo(y.End);
	}
}
