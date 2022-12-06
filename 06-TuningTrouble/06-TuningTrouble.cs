namespace _06_TuningTrouble
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var buffers = File.ReadAllLines(args[0]);

			Console.WriteLine("=== Part One: ===");
			foreach (var buffer in buffers)
			{
				FindMarker(buffer, 4);
			}

			Console.WriteLine("\n\n=== Part Two: ===");
			foreach (var buffer in buffers)
			{
				FindMarker(buffer, 14);
			}
		}

		static int FindMarker(string buffer, int distinctCount)
		{
			Console.WriteLine($"Searching {buffer} for {distinctCount} distinct characters");
			var markerIndex = 0;
			for (var bufferIndex = distinctCount - 1; bufferIndex < buffer.Length; ++bufferIndex)
			{
				var duplicate = false;
				var pattern = buffer[bufferIndex - (distinctCount - 1)].ToString();
				for (var i = distinctCount - 2; i >= 0; --i)
				{
					var nextChar = buffer[bufferIndex - i];
					if (pattern.Contains(nextChar))
					{
						duplicate = true;
						break;
					}
					pattern += nextChar;
				}
				if (!duplicate)
				{
					markerIndex = bufferIndex + 1;
					break;
				}
			}
			Console.WriteLine($"\tMarker: {markerIndex}");
			return markerIndex;
		}
	}
}