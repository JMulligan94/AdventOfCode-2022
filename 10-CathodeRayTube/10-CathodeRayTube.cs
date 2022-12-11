using System.Security.Cryptography;

namespace _10_CathodeRayTube
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var instructions = File.ReadLines(args[0]);
			var x = 1;

			Console.WriteLine($"Sprite position: {GetSpritePositionVisual(x)}");
			var cycleNumber = 1;
			var valueBuffer = new int[2];
			var interestingCycles = new Dictionary<int, int>();
			var crt = new List<string>();
			var currentLine = "";
			foreach (var instruction in instructions)
			{
				Console.WriteLine($"\nStart cycle\t{cycleNumber}: begin executing {instruction}");
				var currentCycleInstruction = instruction;
				var cyclesNeeded = instruction == "noop" ? 1 : 2;
				for (var i = 0; i < cyclesNeeded; ++i)
				{
					// Cycle 20, and every 40 cycles after that
					//	20, 60, 100, 140, 180 etc.
					if ((cycleNumber - 20) % 40 == 0)
					{
						interestingCycles.Add(cycleNumber, x);
					}

					Console.WriteLine($"During cycle\t{cycleNumber}: CRT draws pixel in position {cycleNumber - 1}");

					if (Math.Abs(x - ((cycleNumber -1) % 40)) <= 1)
						currentLine += '#';
					else
						currentLine += ".";

					Console.WriteLine($"Current CRT row: {currentLine}");

					if (cycleNumber % 40 == 0)
					{
						crt.Add(currentLine);
						currentLine = "";
					}

					if (currentCycleInstruction != "noop")
					{
						// Takes TWO cycles to complete then adds v to x
						valueBuffer[1] = int.Parse(currentCycleInstruction.Split(' ')[1]);
						currentCycleInstruction = "noop";
					}
					cycleNumber++;

					if (valueBuffer[0] != 0)
					{
						x += valueBuffer[0];
						valueBuffer[0] = 0;
					}
					if (valueBuffer[1] != 0)
					{
						valueBuffer[0] = valueBuffer[1];
						valueBuffer[1] = 0;
					}
				}
				Console.WriteLine($"End of cycle\t{cycleNumber}: finish executing {instruction} (X is now {x})");

				Console.WriteLine($"Sprite position: {GetSpritePositionVisual(x)}");
			}

			var signalStrength = interestingCycles.Select(c => c.Key * c.Value).Sum();
			Console.WriteLine("\n=== Part One: ===");
			Console.WriteLine($"Signal strength: {signalStrength}");

			Console.WriteLine("\n=== Part Two: ===");
			Console.WriteLine($"CRT Image:");
			foreach(var crtLine in crt)
				Console.WriteLine(crtLine);
		}

		static string GetSpritePositionVisual(int pos)
		{
			var linePos = pos % 40;
			var line = "";
			for (var i = 0; i < linePos - 1; ++i)
				line += ".";
			line += "###";
			for (var i = linePos + 2; i <= 40; ++i)
				line += ".";

			return line;
		}
	}
}