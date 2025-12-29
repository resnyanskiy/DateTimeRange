using AutoFixture;
using AutoFixture.Xunit2;

namespace DateTimeRangeLibrary;

public class ComparersTests
{
	private readonly Fixture _fixture = new();
	private readonly DefaultComparer _comparer = new();

	public ComparersTests()
	{
		// Настраиваем создание DateTime в разумном диапазоне
		_fixture.Customize<DateTime>(c => c.FromFactory<Random>(r => new DateTime(2020, 1, 1).AddDays(r.Next(0, 365))));
	}

	/*
	 * 0123456789012
	 * /----/
	 * /------/
	 *  /----------/
	 *   /------/
	 *     /----/
	 *        /----/
	 * 0123456789012
	 */
	[Fact]
	public void DefaultComparer_GoldenSample()
	{
		// Arrange
		var begin = DateTime.Today;
		DateTimeRange[] expected =
		[
			new(begin, TimeSpan.FromMinutes(5)),
			new(begin, TimeSpan.FromMinutes(7)),
			new(begin.AddMinutes(1), TimeSpan.FromMinutes(11)),
			new(begin.AddMinutes(2), TimeSpan.FromMinutes(7)),
			new(begin.AddMinutes(4), TimeSpan.FromMinutes(5)),
			new(begin.AddMinutes(7), TimeSpan.FromMinutes(5)),
		];
		
		// Act
		var r = new Random();
		var random = expected.OrderBy(_ => r.Next(expected.Length));
		var actual = new SortedSet<DateTimeRange>(random, _comparer);

		// Assert
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	/*
	 * 0123456789012
	 * /----/
	 * /------/
	 *   /------/
	 *     /----/
	 *  /----------/
	 *        /----/
	 * 0123456789012
	 */
	[Fact]
	public void AlternateComparer_GoldenSample()	
	{
		// Arrange
		var begin = DateTime.Today;
		DateTimeRange[] expected =
		[
			new(begin, TimeSpan.FromMinutes(5)),
			new(begin, TimeSpan.FromMinutes(7)),
			new(begin.AddMinutes(2), TimeSpan.FromMinutes(7)),
			new(begin.AddMinutes(4), TimeSpan.FromMinutes(5)),
			new(begin.AddMinutes(1), TimeSpan.FromMinutes(11)),
			new(begin.AddMinutes(7), TimeSpan.FromMinutes(5)),
		];
		
		// Act
		var r = new Random();
		var random = expected.OrderBy(_ => r.Next(expected.Length));
		var actual = new SortedSet<DateTimeRange>(random, new AlternateComparer());

		// Assert
		Assert.Equal(expected.Length, actual.Count);
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ShouldSortBy_Begin_Or_End()
	{
		// Arrange
		var ranges = _fixture.CreateMany<DateTimeRange>(10);
		var sorted = new SortedSet<DateTimeRange>(_comparer);
		foreach (var range in ranges)
		{
			sorted.Add(range);
		}

		// Assert - проверяем сортировку по Begin
		var elements = sorted.ToList();
		for (var i = 0; i < elements.Count - 1; i++)
		{
			Assert.True(elements[i].Begin <= elements[i + 1].Begin);

			// Если начала равны, проверяем сортировку по End
			if (elements[i].Begin == elements[i + 1].Begin)
			{
				Assert.True(elements[i].End <= elements[i + 1].End);
			}
		}
	}

	[Theory, AutoData]
	public void WhenRangesAreIdentical_ShouldReturnZero(DateTimeRange range)
	{
		// Act
		var result = _comparer.Compare(range, range);

		// Assert
		Assert.Equal(0, result);
	}

	[Theory, AutoData]
	public void Compare_ShouldBeTransitive(DateTimeRange range1, DateTimeRange range2, DateTimeRange range3)
	{
		// Act
		var comp12 = _comparer.Compare(range1, range2);
		var comp23 = _comparer.Compare(range2, range3);
		var comp13 = _comparer.Compare(range1, range3);

		// Assert: если range1 < range2 и range2 < range3, то range1 < range3
		if (comp12 < 0 && comp23 < 0)
		{
			Assert.True(comp13 < 0);
		}
	}

	[Theory, AutoData]
	public void Compare_ShouldBeAntisymmetric(DateTimeRange range1, DateTimeRange range2)
	{
		// Act
		var comp12 = _comparer.Compare(range1, range2);
		var comp21 = _comparer.Compare(range2, range1);

		// Assert: если range1 < range2, то range2 > range1
		if (comp12 < 0)
		{
			Assert.True(comp21 > 0);
		}
	}
}
