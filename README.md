Overview
-
The library contains one simple type `DateTimeRange`, which represents an _inclusive_ interval between two points on the timeline. It has some basic features like comparison, validation, etc.
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
	section 276f7b07
		1 :, 20:39.543, 20:39.673
		2 :, 20:41.469, 20:42.318
	section 2e1c4367
		1 :, 20:38.270, 20:38.425
		2 :, 20:40.811, 20:42.389
	section 93cede27
		1 :, 20:38.689, 20:39.856
		2 :, 20:40.585, 20:41.706
	section 8936c10d
		1 :, 20:38.270, 20:38.773
	section ∑ Int
		1 :active, 20:38.270, 20:39.856
		2 :active, 20:40.585, 20:42.389
	section δ Dif
		1 :done, 20:38.270, 20:38.425
		2 :done, 20:38.425, 20:38.689
		3 :done, 20:38.689, 20:38.773
		4 :done, 20:38.773, 20:39.543
		5 :done, 20:39.543, 20:39.673
		6 :done, 20:39.673, 20:39.856
		7 :done, crit, 20:39.856, 20:40.585
		8 :done, 20:40.585, 20:40.811
		9 :done, 20:40.811, 20:41.469
		10 :done, 20:41.469, 20:41.706
		11 :done, 20:41.706, 20:42.318
		12 :done, 20:42.318, 20:42.389
	section x̂ Max
		- :vert, 20:38.270, 0s
		- :vert, 20:38.425, 0s
		I :crit, active, 20:38.270, 20:38.425
		- :vert, 20:38.689, 0s
		- :vert, 20:38.773, 0s
		I :crit, active, 20:38.689, 20:38.773
		- :vert, 20:39.543, 0s
		- :vert, 20:39.673, 0s
		I :crit, active, 20:39.543, 20:39.673
		- :vert, 20:41.469, 0s
		- :vert, 20:41.706, 0s
		I :crit, active, 20:41.469, 20:41.706
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

Example
-
Repository contains [Example](./Example/Program.cs) and [Tests](./Tests/IntersectionTests.Complex.cs), which show how to use the library.
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
1. Add source `github-resnyanskiy`.
```
dotnet nuget add source "https://nuget.pkg.github.com/resnyanskiy/index.json" --name "github-resnyanskiy"
```
2. [Get](https://github.com/settings/tokens) GitHub token with `read:packages` scope for your GitHub account.
3. Set credentials for the source `github-resnyanskiy`.
```
dotnet nuget update source github-resnyanskiy --username resnyanskiy --password YOUR_TOKEN --store-password-in-clear-text
```
4. Add package `DateTimeRange` to your project.
```
dotnet package add DateTimeRange --source https://nuget.pkg.github.com/resnyanskiy/index.json
```
