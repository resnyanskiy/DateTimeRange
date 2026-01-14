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

// Processor: Create report (gantt diagram)
var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Report.md");
using (var writer = new StreamWriter(reportPath))
{
	writer.WriteLine("```mermaid");
	WriteReportHeader();
	
	var ranges = new Queue<DateTimeRange>();
	
	// ranges
	foreach (var sensor in storage.ToLookup(x => x.SensorId))
	{
		// Get ranges and sort them
		var signals = sensor.ToDictionary(s => s.Timestamp, s => s.Value);
		var query = DateTimeRange.Create(signals, 5).Where(r => r.End < DateTime.MaxValue);
		var set = new SortedSet<DateTimeRange>(query, new DefaultComparer());
		
		// Write diagram
		if (set.Count > 0)
		{
			writer.WriteLine('\t' + $"section {sensor.Key.ToShortString()}");
			var rangeInSection = 1;
			foreach (var range in set)
			{
				WriteGanttTask(range, $"{rangeInSection++}");
				
				// Save for analysis
				ranges.Enqueue(range);
			}
		}
	}
	
	// merge
	writer.WriteLine('\t' + "section ∑ Int");
	var merges = ranges.Merge();
	foreach (var range in merges.Select((range, index) => (Data: range, Index: index + 1)))
	{
		WriteGanttTask(range.Data, $"{range.Index}", "active");
	}
	
	// slice
	writer.WriteLine('\t' + "section δ Dif");
	var slices = ranges.Slice();
	foreach (var range in slices.Select((range, index) => (Data: range, Index: index + 1)))
	{
		WriteGanttTask(range.Data, $"{range.Index}", "done");
	}
	
	// intersections
	writer.WriteLine('\t' + "section x̂ Max");
	var intersections = ranges.Intersections();
	foreach (var range in intersections)
	{
		WriteVerticalLines(range);
		WriteGanttTask(range, "I", "crit, active");
	}
	
	writer.WriteLine("```");

	void WriteReportHeader()
	{
		writer.WriteLine("---");
		writer.WriteLine("displayMode: compact");
		writer.WriteLine("config:");
		writer.WriteLine(" themeCSS:");
		writer.WriteLine(Theme());
		writer.WriteLine(" gantt:");
		writer.WriteLine("  numberSectionStyles: 1");
		writer.WriteLine("---");
	
		writer.WriteLine("gantt");
		writer.WriteLine('\t' + "title Sensors");
		writer.WriteLine('\t' + "dateFormat mm:ss.SSS");
		writer.WriteLine('\t' + "axisFormat %S:%L");
		
		string Theme() => 
			"""
		      "\n
		      rect[id^=vert] { height: calc(100% - 50px) ; transform: translate(0px, 30px); y: 0; width: 1px; stroke: none; fill: red; }\n
		      text[id^=vert] { display: none }"
		   """;
	}
	
	void WriteGanttTask(DateTimeRange range, string name, string? type = null)
	{
		var begin = range.Begin.ToString("mm:ss.fff");
		var end = range.End.ToString("mm:ss.fff");
		
		writer.WriteLine("\t\t" + $"{name} :{type ?? string.Empty}, {begin}, {end}");
	}
	
	void WriteVerticalLines(DateTimeRange range)
	{
		var begin = range.Begin.ToString("mm:ss.fff");
		var end = range.End.ToString("mm:ss.fff");

		writer.WriteLine("\t\t" + $"- :vert, {begin}, 0s");
		writer.WriteLine("\t\t" + $"- :vert, {end}, 0s");
	}
}

Console.SetCursorPosition(0, NUMBER_OF_SENSORS + 3);
Console.WriteLine($"file://{reportPath}");
