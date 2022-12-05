using System.Xml.Serialization;

namespace _05_SupplyStacks
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);

			// Find index of first instruction line 
			//  (next line after an empty line)
			var instructionStartIndex = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).Count() + 1;

			var partOneStacks = ParseInputIntoStacks(args[0]);

			Console.WriteLine("\n\n=== Part One: ===");
			Console.WriteLine("Initial stacks:");
			PrintStacks(ref partOneStacks);

			foreach (var instruction in lines.Skip(instructionStartIndex))
			{
				RearrangeCrates(instruction, ref partOneStacks);
			}

			var partOneMsg = "";
			foreach(var stack in partOneStacks)
			{
				partOneMsg += stack.Peek();
			}
			Console.WriteLine($"Message from top crates: {partOneMsg}");

			Console.WriteLine("\n\n=== Part Two: ===");
			var partTwoStacks = ParseInputIntoStacks(args[0]);
			Console.WriteLine("Initial stacks:");
			PrintStacks(ref partTwoStacks);

			foreach (var instruction in lines.Skip(instructionStartIndex))
			{
				RearrangeCrates_Part2(instruction, ref partTwoStacks);
			}

			var partTwoMsg = "";
			foreach (var stack in partTwoStacks)
			{
				partTwoMsg += stack.Peek();
			}
			Console.WriteLine($"Message from top crates: {partTwoMsg}");
		}

		static List<Stack<char>> ParseInputIntoStacks(string input)
		{
			var lines = File.ReadAllLines(input);

			// Find index of first instruction line 
			//  (next line after an empty line)
			var instructionStartIndex = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).Count() + 1;

			var stacks = new List<Stack<char>>();
			// Work from bottom crate line to top to construct the initial state of crates
			foreach (var line in lines.Take(instructionStartIndex - 2).Reverse())
			{
				var stackIndex = 0;
				for (var i = 0; i < line.Length; i += 4)
				{
					//Console.WriteLine($"{i}: '{line.Substring(i, 3)}'");
					if (line[i + 1] != ' ')
					{
						if (stackIndex >= stacks.Count)
							stacks.Add(new Stack<char>());

						stacks[stackIndex].Push(line[i + 1]);
					}
					stackIndex++;
				}
			}
			return stacks;
		}

		static void PrintStacks(ref List<Stack<char>> stacks)
		{
			for (var i = 0; i < stacks.Count; ++i)
			{
				Console.WriteLine($"{i+1}\t|{string.Join(',', stacks[i].Reverse())}");
			}
		}

		static void RearrangeCrates(string instruction, ref List<Stack<char>> stacks)
		{
			Console.WriteLine($"\n{instruction}");
			var instructionTokens = instruction.Split(' ');
			var quantity = int.Parse(instructionTokens[1]);
			var from = int.Parse(instructionTokens[3]);
			var to = int.Parse(instructionTokens[5]);

			Console.WriteLine($"\t {from}->{to}\t\t{quantity} times");

			// Move crates individually
			for (var i = 0; i< quantity; ++i)
			{
				stacks[to-1].Push(stacks[from-1].Pop());
			}

			Console.WriteLine("Stacks:");
			PrintStacks(ref stacks);
		}
		
		static void RearrangeCrates_Part2(string instruction, ref List<Stack<char>> stacks)
		{
			Console.WriteLine($"\n{instruction}");
			var instructionTokens = instruction.Split(' ');
			var quantity = int.Parse(instructionTokens[1]);
			var from = int.Parse(instructionTokens[3]);
			var to = int.Parse(instructionTokens[5]);

			Console.WriteLine($"\t {from}->{to}\t\t{quantity} times");

			// Move crates as single stack
			var stackToMove = new Stack<char>();
			for (var i = 0; i< quantity; ++i)
			{
				stackToMove.Push(stacks[from-1].Pop());
			}

			for (var i = 0; i < quantity; ++i)
			{
				stacks[to - 1].Push(stackToMove.Pop());
			}

			Console.WriteLine("Stacks:");
			PrintStacks(ref stacks);
		}
	}
}