Overview
-
The library contains one simple type `DateTimeRange`, which represents an _inclusive_ interval between two points on the timeline. It has some basic features like comparison, validation, deconstructing, etc.
```csharp
record struct DateTimeRange
{
    public DateTime Begin { get; }
    public DateTime End { get; }
}
```

But the most valuable part is [Extensions](./Library/Extensions.cs), allowing to do things like this:
```mermaid
---
displayMode: compact
config:
 themeCSS:
   "\n
   rect[id^=vert] { height: calc(100% - 50px) ; transform: translate(0px, 30px); y: 0; width: 1px; stroke: none; fill: red; }\n
   text[id^=vert] { display: none }"
 gantt:
  numberSectionStyles: 1
---
gantt
	title Sensors
	dateFormat mm:ss.SSS
	axisFormat %S:%L
	section 0dad861a
		1 :, 13:37.913, 13:39.402
		2 :, 13:43.170, 13:43.435
	section 636a559e
		1 :, 13:36.097, 13:37.103
		2 :, 13:39.798, 13:41.421
		3 :, 13:42.127, 13:43.751
		4 :, 13:44.927, 13:45.626
	section 505db785
		1 :, 13:37.423, 13:37.901
		2 :, 13:39.578, 13:39.923
		3 :, 13:40.568, 13:40.848
		4 :, 13:42.240, 13:42.435
		5 :, 13:43.588, 13:44.001
		6 :, 13:44.781, 13:45.617
	section 9167c3d9
		1 :, 13:36.960, 13:37.790
		2 :, 13:40.380, 13:41.016
		3 :, 13:41.841, 13:42.440
		4 :, 13:45.017, 13:45.439
	section ∑ Int
		1 :active, 13:36.097, 13:37.901
		2 :active, 13:37.913, 13:39.402
		3 :active, 13:39.578, 13:41.421
		4 :active, 13:41.841, 13:44.001
		5 :active, 13:44.781, 13:45.626
	section δ Dif
		1 :done, 13:36.097, 13:36.960
		2 :done, 13:36.960, 13:37.103
		3 :done, 13:37.103, 13:37.423
		4 :done, 13:37.423, 13:37.790
		5 :done, 13:37.790, 13:37.901
		6 :done, 13:37.901, 13:37.913
		7 :done, 13:37.913, 13:39.402
		8 :done, 13:39.402, 13:39.578
		9 :done, 13:39.578, 13:39.798
		10 :done, 13:39.798, 13:39.923
		11 :done, 13:39.923, 13:40.380
		12 :done, 13:40.380, 13:40.568
		13 :done, 13:40.568, 13:40.848
		14 :done, 13:40.848, 13:41.016
		15 :done, 13:41.016, 13:41.421
		16 :done, 13:41.421, 13:41.841
		17 :done, 13:41.841, 13:42.127
		18 :done, 13:42.127, 13:42.240
		19 :done, 13:42.240, 13:42.435
		20 :done, 13:42.435, 13:42.440
		21 :done, 13:42.440, 13:43.170
		22 :done, 13:43.170, 13:43.435
		23 :done, 13:43.435, 13:43.588
		24 :done, 13:43.588, 13:43.751
		25 :done, 13:43.751, 13:44.001
		26 :done, 13:44.001, 13:44.781
		27 :done, 13:44.781, 13:44.927
		28 :done, 13:44.927, 13:45.017
		29 :done, 13:45.017, 13:45.439
		30 :done, 13:45.439, 13:45.617
		31 :done, 13:45.617, 13:45.626
	section x̂ Max
		- :vert, 13:36.960, 0s
		- :vert, 13:37.103, 0s
		I :crit, active, 13:36.960, 13:37.103
		- :vert, 13:37.423, 0s
		- :vert, 13:37.790, 0s
		I :crit, active, 13:37.423, 13:37.790
		- :vert, 13:39.798, 0s
		- :vert, 13:39.923, 0s
		I :crit, active, 13:39.798, 13:39.923
		- :vert, 13:40.568, 0s
		- :vert, 13:40.848, 0s
		I :crit, active, 13:40.568, 13:40.848
		- :vert, 13:42.240, 0s
		- :vert, 13:42.435, 0s
		I :crit, active, 13:42.240, 13:42.435
		- :vert, 13:43.170, 0s
		- :vert, 13:43.435, 0s
		I :crit, active, 13:43.170, 13:43.435
		- :vert, 13:43.588, 0s
		- :vert, 13:43.751, 0s
		I :crit, active, 13:43.588, 13:43.751
		- :vert, 13:45.017, 0s
		- :vert, 13:45.439, 0s
		I :crit, active, 13:45.017, 13:45.439
```

Current version supports the following methods (feel free to [request/propose/discuss](https://github.com/resnyanskiy/DateTimeRange/discussions) any other useful extensions):
```csharp
// Merges overlapping ranges in a collection of non-overlapping ranges.
// The example result is "Int" (meaning "integration") row on the diagram above.
IEnumerable<DateTimeRange> Merge(this IEnumerable<DateTimeRange> ranges)
```
```csharp
// Slices ranges into distinct adjacent ranges based on unique boundary points.
// The example result is "Dif" (meaning "differential") row on the diagram above.
IEnumerable<DateTimeRange> Slice(this IEnumerable<DateTimeRange> ranges)
```
```csharp
// Calculates all intersections between all provided ranges.
// The example result is "Max" (meaning "signals strength") row on the diagram above.
IEnumerable<DateTimeRange> Intersections(this IEnumerable<DateTimeRange> ranges)
```

<details>
<summary>Key features of the library (the LLM's "take")</summary>

- **Efficient algorithms**: Optimized for performance with large datasets.
- **LINQ-compatible**: Works seamlessly with LINQ and other .NET collections.
- **Memory efficient**: Uses iterators for lazy evaluation where possible.
- **Code quality**: The library includes comprehensive unit tests.
</details>

Big O
-
|       | DateTimeRange.Create | Merge           | Slice        | Intersections         |
|------:|----------------------|-----------------|--------------|-----------------------|
|  Time | `O(n)`               | `O(n log n)`⁽¹⁾ | `O(n log n)` | `O(n log n)`..`O(n²)` |
| Space | `O(1)`               | `O(n)`          | `O(n)`       | `O(n)`⁽²⁾             |

⁽¹⁾Time complexity is `O(n)` if the input is already sorted.

⁽²⁾Space complexity is `O(1)` if the input is a sorted array.

Docs
-
The library's current API is very simple, so no guidelines is needed. Repository contains [Example](./Example/Program.cs) and [Tests](./Tests/IntersectionTests.Complex.cs), which show how to use the library.
```csharp
using DateTimeRangeLibrary;

Dictionary<DateTime, double> temperatureOutside;
Dictionary<DateTime, double> temperatureInside;

// Create ranges where temperature is above threshold
IEnumerable<DateTimeRange> hotOutside = DateTimeRange.Create(temperatureOutside, 20.0);
IEnumerable<DateTimeRange> hotInside = DateTimeRange.Create(temperatureInside, 20.0);

var hotPeriodsQuery = hotOutside.Concat(hotInside).Where(r => r.End < DateTime.MaxValue);

// Find periods when there was hot inside and outside at the same time
var maxHotPeriods = hotPeriodsQuery.Intersections();
```

NuGet Package
-
`DateTimeRange` is available on [GitHub Packages](https://github.com/users/resnyanskiy/packages/nuget/). To consume:
1. [Get](https://github.com/settings/tokens) token with `read:packages` scope for your GitHub account.
2. Add source `github-resnyanskiy`:
```
dotnet nuget add source "https://nuget.pkg.github.com/resnyanskiy/index.json" --name "github-resnyanskiy"
```
3. Set credentials for the source `github-resnyanskiy`:
```
dotnet nuget update source github-resnyanskiy --username resnyanskiy --password YOUR_TOKEN --store-password-in-clear-text
```
4. Add package `DateTimeRanges` to your project:
```
dotnet package add DateTimeRanges --source https://nuget.pkg.github.com/resnyanskiy/index.json
```
