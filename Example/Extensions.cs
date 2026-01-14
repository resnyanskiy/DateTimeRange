namespace Example;

internal static class Extensions
{
	extension(Guid guid)
	{
		public string ToShortString() => guid.ToString("N")[..8];
	}
}
