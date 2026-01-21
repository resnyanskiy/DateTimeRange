using System.Collections.Concurrent;

// Consumer: Save signals
var stream = new BlockingCollection<Signal>();
var storage = new Queue<Signal>();
var saver = Task.Run(() =>
{
	while (!stream.IsAddingCompleted)
	{
		foreach (var signal in stream.GetConsumingEnumerable())
		{
			storage.Enqueue(signal);
		}
	}
});

// Producer: Generate signals
const int NUMBER_OF_SENSORS = 4;
var sensors = new List<Task>(NUMBER_OF_SENSORS);
var consoleLock = new Lock();
using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
{
	for (var i = 0; i < NUMBER_OF_SENSORS; i++)
	{
		sensors.Add(GenerateSignals(cts.Token));
	}

	// press 'Enter' or wait 10 seconds to stop
	var cancelTask = Task.Run(() => { Console.ReadLine(); cts.Cancel(); });
	var timeoutTask = Task.Delay(Timeout.Infinite, cts.Token);
	await Task.WhenAny(cancelTask, timeoutTask);
}
async Task GenerateSignals(CancellationToken ct) 
{
	// save console output position
	Console.WriteLine();
	var cursorTop = Console.CursorTop;
	
	var sensorId = Guid.NewGuid();
	var random = new Random();
	while (!ct.IsCancellationRequested)
	{
		var signal = new Signal
		{
			Timestamp = DateTime.Now,
			Value = random.Next(1, 10),
			SensorId = sensorId
		};
		
		stream.Add(signal);
		
		// show status in console
		lock (consoleLock)
		{
			Console.SetCursorPosition(0, cursorTop);
			Console.Write($"Sensor[{sensorId.ToShortString()}]: {signal.Timestamp:mm:ss} {signal.Value}");
		}
		
		// send new signal after random delay
		await Task.Delay(random.Next(120, 1000));
	}
	
	stream.CompleteAdding();
}

await Task.WhenAll(sensors);
await saver;

// Processor: Create report
var ranges = new List<(Guid SensorId, DateTimeRange Range)>();
var all = new Queue<DateTimeRange>();

// get ranges from signals
foreach (var sensor in storage.ToLookup(x => x.SensorId))
{
	var signals = sensor.ToDictionary(s => s.Timestamp, s => s.Value); // get ranges and sort them
	var query = DateTimeRange.Create(signals, 5).Where(r => r.End < DateTime.MaxValue);
	var set = new SortedSet<DateTimeRange>(query, new DefaultComparer());
	
	if (set.Count > 0)
	{
		foreach (var range in set)
		{
			ranges.Add((sensor.Key, range));
			all.Enqueue(range); // save for analysis as single set of ranges
		}
	}
}

// analysis
var merges = all.Merge();
var slices = all.Slice();
var intersections = all.Intersections().OrderBy(x => x.End - x.Begin).ToArray();

var intervalTree = new IntervalTree(all);
var intersectingRanges = intervalTree.SearchIntersections(intersections.Last()).ToHashSet(); // ranges intersecting max intersection

// create report
var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Report.md");
using (var writer = new StreamWriter(reportPath))
{
	// Ranges
	writer.WriteLine("```mermaid");
	
	#region Mermaid Gantt Header

	writer.WriteLine("---");
	writer.WriteLine("displayMode: compact");
	writer.WriteLine("config:");
	writer.WriteLine(" themeCSS:");
	writer.WriteLine(Theme1());
	writer.WriteLine(" gantt:");
	writer.WriteLine("  numberSectionStyles: 1");
	writer.WriteLine("---");
	
	writer.WriteLine("gantt");
	writer.WriteLine('\t' + "title Sensors");
	writer.WriteLine('\t' + "dateFormat mm:ss.SSS");
	writer.WriteLine('\t' + "axisFormat %S:%L");

	string Theme1() => 
		"""
		   "\n
		   rect[id^=vert] { height: calc(100% - 50px) ; transform: translate(0px, 30px); y: 0; width: 1px; stroke: none; fill: red; }\n
		   text[id^=vert] { display: none }"
		""";
	
	#endregion

	// ranges sections
	var rangeTitle = new Dictionary<DateTimeRange, string>(all.Count); //TODO could be same range from different sensors
	foreach (var sensor in ranges.ToLookup(x => x.SensorId, x => x.Range))
	{
		writer.WriteLine('\t' + $"section {sensor.Key.ToShortString()}");
		var rangeInSection = 1;
		foreach (var range in sensor)
		{
			rangeTitle[range] = $"{sensor.Key.ToShortString()} {rangeInSection}"; 
			writer.WriteMermaidGanttTask(range, $"{rangeInSection++}", intersectingRanges.Contains(range) ? "crit, active" : "active");
		}		
	}
	
	// merge section
	writer.WriteMermaidGanttSection(merges, "∑ Int");
	
	// slices section
	writer.WriteMermaidGanttSection(slices, "δ Dif", "done");
	
	// intersections section
	writer.WriteLine('\t' + "section x̂ Max");
	foreach (var range in intersections)
	{
		var begin = range.Begin.ToString("mm:ss.fff");
		var end = range.End.ToString("mm:ss.fff");

		writer.WriteLine("\t\t" + $"- :vert, {begin}, 0s");
		writer.WriteLine("\t\t" + $"- :vert, {end}, 0s");
		writer.WriteMermaidGanttTask(range, "I", "crit, done");
	}
	
	writer.WriteLine("```");

	writer.WriteLine();
	
	// Interval tree
	writer.WriteLine("```mermaid");
	writer.WriteLine("---");
	writer.WriteLine("title: Interval tree");
	writer.WriteLine("---");
	writer.WriteMermaidGraph(intervalTree, node => rangeTitle[node.Range]);	
	writer.WriteLine("```");
}

Console.SetCursorPosition(0, NUMBER_OF_SENSORS + 3);
Console.WriteLine($"file://{reportPath}");
