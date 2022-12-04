
namespace _04_CampCleanup
{
	class Program
	{
		static void Main(string[] args)
		{
			var assignments = File.ReadAllLines(args[0]);

			var fullOverlaps = 0;
			var partialOverlaps = 0;
			foreach (var item in assignments) 
			{
				var assignmentA = item.Split(',')[0];
				var assignmentB = item.Split(',')[1];
				Console.WriteLine($"\tA:{assignmentA} B:{assignmentB}");

				if (HasFullOverlap(assignmentA, assignmentB))
				{
					Console.WriteLine("\t\tFull overlap found!");
					fullOverlaps++;
				}
				if (HasAnyOverlap(assignmentA, assignmentB))
				{
					Console.WriteLine("\t\tPartial overlap found!");
					partialOverlaps++;
				}
			}

			Console.WriteLine("\n\n=== Part One: ===");
			Console.WriteLine($"Number of full overlaps: {fullOverlaps}");

			Console.WriteLine("\n\n=== Part Two: ===");
			Console.WriteLine($"Number of partial overlaps: {partialOverlaps}");
		}

		// Check if one assignment in a pair is completely contained within the other assignment's section range
		static bool HasFullOverlap(string assignmentA, string assignmentB)
		{
			var lowerA = int.Parse(assignmentA.Split('-')[0]);
			var higherA = int.Parse(assignmentA.Split('-')[1]);
			var lowerB = int.Parse(assignmentB.Split('-')[0]);
			var higherB = int.Parse(assignmentB.Split('-')[1]);

			if (lowerB <= lowerA && higherA <= higherB)
			{
				// A completely overlaps with B
				return true;
			}
			if (lowerA <= lowerB && higherB <= higherA)
			{
				// B completely overlaps with A
				return true;
			}
			return false;
		}

		// Check if assignments have at least one section of overlap
		static bool HasAnyOverlap(string assignmentA, string assignmentB)
		{
			var lowerA = int.Parse(assignmentA.Split('-')[0]);
			var higherA = int.Parse(assignmentA.Split('-')[1]);
			var lowerB = int.Parse(assignmentB.Split('-')[0]);
			var higherB = int.Parse(assignmentB.Split('-')[1]);

			if (higherA < lowerB || higherB < lowerA)
			{
				return false;
			}
			return true;
		}
	}
}