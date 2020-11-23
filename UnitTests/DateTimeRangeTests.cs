using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassLibrary
{
  [TestClass]
  public class DateTimeRangeTests
  {
    [TestMethod]
    public void EqualityTest()
    {
      // arrange
      var now = DateTime.Now;
      var range = new DateTimeRange(now, TimeSpan.FromHours(-1));
      var equal = new DateTimeRange(range.End, range.Begin.ToUniversalTime());
      var other = range + TimeSpan.FromMinutes(1);

      // assert: Operators
      Assert.IsTrue(range == equal);
      Assert.IsTrue(range != other);

      // assert: Equals(object) method
      Assert.AreEqual(range, equal);
      Assert.AreNotEqual(range, other);

      // assert: GetHashCode() method
      var set = new HashSet<DateTimeRange>();
      Assert.IsTrue(set.Add(range));
      Assert.IsFalse(set.Add(equal));
      Assert.IsTrue(set.Add(other));
    }

    [TestMethod]
    public void CreateFromPulseTest()
    {
      /*
       * input:    + - + - + + - -
       * output:   |-| |-| |---|
       *
       */

      // arrange
      var begin = DateTime.Today;
      var pulse = new Dictionary<DateTime, bool>
      {
        [begin.AddMinutes(1)] = true,
        [begin.AddMinutes(2)] = false,
        [begin.AddMinutes(3)] = true,
        [begin.AddMinutes(4)] = false,
        [begin.AddMinutes(5)] = true,
        [begin.AddMinutes(6)] = true,
        [begin.AddMinutes(7)] = false,
        [begin.AddMinutes(8)] = false
      };

      // act
      var ranges = DateTimeRange.Create(pulse).ToArray();

      // assert
      Assert.AreEqual(3, ranges.Count());
      Assert.AreEqual(new DateTimeRange(begin.AddMinutes(1), begin.AddMinutes(2)), ranges[0]);
      Assert.AreEqual(new DateTimeRange(begin.AddMinutes(3), begin.AddMinutes(4)), ranges[1]);
      Assert.AreEqual(new DateTimeRange(begin.AddMinutes(5), begin.AddMinutes(7)), ranges[2]);
    }

    [TestMethod]
    public void SliceTest()
    {
      /*
       |-----|
         |-|
           |---|
                 |-----|
                       |-----|
                    |--------|

       |-|-|-|-| |--|--|-----|
       */

      // arrange
      var begin = DateTime.Now.Date;
      var ranges = new[]
      {
          new DateTimeRange(begin,              begin.AddHours(12)),
          new DateTimeRange(begin.AddHours(2),  begin.AddHours(10)),
          new DateTimeRange(begin.AddHours(10), begin.AddHours(14)),
          new DateTimeRange(begin.AddHours(18), begin.AddHours(20)),
          new DateTimeRange(begin.AddHours(20), begin.AddHours(22)),
          new DateTimeRange(begin.AddHours(19), begin.AddHours(22)),
      };

      // act
      var slices = ranges.Slice().ToArray();

      // assert
      Assert.AreEqual(7, slices.Length);
      AssertSliceEqual(1, 0,  2);
      AssertSliceEqual(2, 2,  10);
      AssertSliceEqual(3, 10, 12);
      AssertSliceEqual(4, 12, 14);
      AssertSliceEqual(5, 18, 19);
      AssertSliceEqual(6, 19, 20);
      AssertSliceEqual(7, 20, 22);

      void AssertSliceEqual(int s, int b, int e) => Assert.AreEqual(new DateTimeRange(begin.AddHours(b), begin.AddHours(e)), slices[s - 1]);
    }

    [TestMethod]
    public void IntersectTest()
    {
      // arrange
      var now = DateTime.Now;
      var range = new DateTimeRange(now, TimeSpan.FromHours(1));                                   // 00:00 - 01:00
      var rangeBefore = new DateTimeRange(range.Begin.AddMinutes(-20), TimeSpan.FromMinutes(10));  // -0:20 - -0:10
      var rangeInside = new DateTimeRange(range.Begin.AddMinutes(20), range.End.AddMinutes(-10));  // 00:20 - 00:50
      var rangeInsideAndAfter = new DateTimeRange(range.Begin.AddMinutes(30), null);               // 00:30 - Infinity

      // act
      var intersections = range.Intersect(rangeInsideAndAfter, rangeBefore, rangeInside);

      // assert
      Assert.AreEqual(new DateTimeRange(rangeInsideAndAfter.Begin, range.End), intersections.First()); // 00:30 - 01:00
      Assert.AreEqual(new DateTimeRange(rangeInside.Begin, rangeInside.End), intersections.Last());    // 00:20 - 00:50
    }

    [TestMethod]
    public void ExceptTest()
    {
      // arrange
      var now = DateTime.Now;
      var range = new DateTimeRange(now, TimeSpan.FromHours(1));                                  // 00:00 - 01:00
      var rangeInsideAndAfter = new DateTimeRange(range.Begin.AddMinutes(40), null);              // 00:40 - Infinity
      var rangeInside = new DateTimeRange(range.Begin.AddMinutes(20), TimeSpan.FromMinutes(10));  // 00:20 - 00:30
      var rangeBefore = new DateTimeRange(range.Begin.AddMinutes(-10), TimeSpan.FromMinutes(20)); // 23:50 - 00:10

      // assert: return range
      AssertReturnRange(range.Except(null));
      AssertReturnRange(range.Except(new DateTimeRange[1]));
      AssertReturnRange(range.Except(new DateTimeRange(range.End.AddMinutes(1), TimeSpan.FromMinutes(1))));

      // assert: cross bounds
      var extract1 = range.Except(rangeInsideAndAfter, rangeInside, rangeBefore);
      Assert.AreEqual(new DateTimeRange(rangeBefore.End, rangeInside.Begin), extract1.First());        // 00:10 - 00:20
      Assert.AreEqual(new DateTimeRange(rangeInside.End, rangeInsideAndAfter.Begin), extract1.Last()); // 00:30 - 00:40

      // assert: cross begin
      var extract2 = range.Except(rangeBefore);
      Assert.AreEqual(new DateTimeRange(rangeBefore.End, range.End), extract2.First()); // 00:10 - 01:00

      // assert: cross end
      var extract3 = range.Except(rangeInsideAndAfter);
      Assert.AreEqual(new DateTimeRange(range.Begin, rangeInsideAndAfter.Begin), extract3.First()); // 00:00 - 00:40

      // assert: inside
      var extract4 = range.Except(rangeInside);
      Assert.AreEqual(new DateTimeRange(range.Begin, rangeInside.Begin), extract4.First()); // 00:00 - 00:20
      Assert.AreEqual(new DateTimeRange(rangeInside.End, range.End), extract4.Last());      // 00:30 - 01:00

      void AssertReturnRange(IEnumerable<DateTimeRange> extract)
      {
        Assert.AreEqual(range, extract.First());
      }
    }
  }
}
