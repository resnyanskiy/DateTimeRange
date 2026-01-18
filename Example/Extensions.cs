namespace Example;

internal static class Extensions
{
	extension(Guid guid)
	{
		public string ToShortString() => guid.ToString("N")[..8];
	}

	extension(StreamWriter writer)
	{
		public void WriteMermaidGraph(IntervalTree tree)
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
             
				var begin = node.Range.Begin.ToString("ss.fff");
				var end = node.Range.End.ToString("ss.fff");
		
				writer.WriteLine('\t' + $"N{id}[\"{begin} - {end}<br/>{node.Height}\"]");
				
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
