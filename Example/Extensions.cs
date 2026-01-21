namespace Example;

internal static class Extensions
{
	extension(Guid guid)
	{
		public string ToShortString() => guid.ToString("N")[..8];
	}

	extension(StreamWriter writer)
	{
		public void WriteMermaidGanttTask(DateTimeRange range, string name, string? type = null)
		{
			var begin = range.Begin.ToString("mm:ss.fff");
			var end = range.End.ToString("mm:ss.fff");
		
			writer.WriteLine("\t\t" + $"{name} :{type ?? string.Empty}, {begin}, {end}");
		}
		
		public void WriteMermaidGanttSection(
			IEnumerable<DateTimeRange> ranges, string sectionName,
			string? taskType = null, string? taskName = null)
		{
			writer.WriteLine('\t' + $"section {sectionName}");
			foreach (var range in ranges.Select((range, index) => (Data: range, Index: index + 1)))
			{
				writer.WriteMermaidGanttTask(range.Data, taskName ?? $"{range.Index}", taskType);
			}
		}
		
		public void WriteMermaidGraph(IntervalTree tree, Func<IntervalTree.Node, string> getNodeTitle)
		{
			if (tree.Root == null)
				return;
			
			writer.WriteLine("graph");
			
			// BFS
			var queue = new Queue<(IntervalTree.Node node, int id)>();
			var nextId = 1;
			queue.Enqueue((tree.Root, nextId++));
			while (queue.Count > 0)
			{
				var (node, id) = queue.Dequeue();
             
				writer.WriteLine('\t' + $"N{id}[\"{getNodeTitle(node)}\"]");
				
				if (node.Left != null)
				{
					var leftId = nextId++;
					queue.Enqueue((node.Left, leftId));
					WriteRelation(id, leftId);
				}
             
				if (node.Right != null)
				{
					var rightId = nextId++;
					queue.Enqueue((node.Right, rightId));
					WriteRelation(id, rightId);
				}
			}
			
			void WriteRelation(int from, int to) => writer.WriteLine('\t'+ $"N{from} --> N{to}");
		}
	}
}
