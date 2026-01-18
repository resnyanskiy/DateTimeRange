namespace DateTimeRangeLibrary;

// https://en.wikipedia.org/wiki/Interval_tree
public class IntervalTree
{
    public class Node(DateTimeRange range)
    {
        public Node? Left { get; internal set; }
        public Node? Right { get; internal set; }
        public int Height { get; private set; } = 1;

        public DateTimeRange Range { get; } = range;

        internal DateTime MaxEnd { get; private set; } = range.End;
        
        private static int GetHeight(Node? node) => node?.Height ?? 0;

        internal int GetBalance() => GetHeight(Left) - GetHeight(Right);
        
        internal void Update()
        {
            Height = 1 + Math.Max(GetHeight(Left), GetHeight(Right));

            MaxEnd = Range.End; // Reset to base value

            if (Left != null && MaxEnd < Left.MaxEnd)
            {
                MaxEnd = Left.MaxEnd;
            }

            if (Right != null && MaxEnd < Right.MaxEnd)
            {
                MaxEnd = Right.MaxEnd;
            }
        }
    }
    
    private static Node Balance(Node node)
    {
	    node.Update();

	    switch (node.GetBalance())
	    {
		    case > 1:
		    {
			    if (node.Left!.GetBalance() < 0)
			    {
				    node.Left = RotateLeft(node.Left);
			    }

			    return RotateRight(node);
		    }
		    case < -1:
		    {
			    if (node.Right!.GetBalance() > 0)
			    {
				    node.Right = RotateRight(node.Right);
			    }

			    return RotateLeft(node);
		    }
		    default:
			    return node;
	    }
    }

    private static Node RotateRight(Node pivot)
    {
	    var root = pivot.Left!;
	    var right = root.Right;

	    root.Right = pivot;
	    pivot.Left = right;

	    pivot.Update();
	    root.Update();

	    return root;
    }

    private static Node RotateLeft(Node pivot)
    {
	    var root = pivot.Right!;
	    var left = root.Left;

	    root.Left = pivot;
	    pivot.Right = left;

	    pivot.Update();
	    root.Update();

	    return root;
    }
    
    public IntervalTree(IEnumerable<DateTimeRange> ranges)
    {
        foreach (var range in ranges)
        {
            Root = Insert(Root, range);
        }
    }

    public Node? Root { get; }

    private Node Insert(Node? node, DateTimeRange range)
    {
        if (node == null)
            return new Node(range);

        if (range.Begin < node.Range.Begin)
        {
            node.Left = Insert(node.Left, range);
        }
        else
        {
            node.Right = Insert(node.Right, range);
        }

        return Balance(node);
    }
    
    /// <summary>
    /// Returns all ranges that intersect with provided range.
    /// </summary>
    /// <remarks>
    /// It returns `intersecting ranges`, not `intersections`.
    /// </remarks>
    /// <param name="range">The range to check for intersections.</param>
    /// <returns>Enumeration of intersecting ranges.</returns>    
    public IEnumerable<DateTimeRange> SearchIntersections(DateTimeRange range)
    {
        if (Root == null)
            yield break;

        var stack = new Stack<Node>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            if (node.MaxEnd < range.Begin)
                continue;

            if (node.Range.Begin <= range.End && range.Begin <= node.Range.End)
                yield return node.Range;

            if (node.Left != null && range.Begin <= node.Left.MaxEnd)
                stack.Push(node.Left);

            if (node.Right != null && node.Range.Begin <= range.End)
                stack.Push(node.Right);
        }
    }
}
