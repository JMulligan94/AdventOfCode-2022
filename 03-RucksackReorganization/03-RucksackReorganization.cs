
namespace _03_RucksackReorganization
{
	class Program
	{
		static void Main(string[] args)
		{
			var rucksacks = File.ReadAllLines(args[0]);

			Console.WriteLine("=== Part One: ===");
			var partOneScore = 0;
			foreach(var rucksack in rucksacks) 
			{
				partOneScore += GetRusksackItemTypeScores_Individual(rucksack);
			}
			Console.WriteLine($"Rucksack item type score sum (individual): {partOneScore}");

			Console.WriteLine("\n\n=== Part Two: ===");
			var partTwoScore = 0;
			for (var i = 0; i < rucksacks.Length; i+=3)
			{
				partTwoScore += GetRusksackItemTypeScores_Group(rucksacks[i], rucksacks[i+1], rucksacks[i+2]);
			}
			Console.WriteLine($"Rucksack item type score sum (group): {partTwoScore}");
		}

		static int GetRusksackItemTypeScores_Individual(string rucksack)
		{
			Console.WriteLine($"Rucksack: {rucksack}");
			var compartmentA = rucksack.Substring(0, rucksack.Length / 2);
			var compartmentB = rucksack.Substring(rucksack.Length / 2);
			Console.WriteLine($"\tA: {compartmentA}");
			Console.WriteLine($"\tB: {compartmentB}");

			var commonItem = compartmentA.First(compartmentB.Contains);
			var commonItemScore = char.IsUpper(commonItem) ? commonItem - 'A' + 27 : commonItem - 'a' + 1;
			Console.WriteLine($"\tCommon item: {commonItem} ({commonItemScore})");

			return commonItemScore;
		}

		static int GetRusksackItemTypeScores_Group(string rucksack1, string rucksack2, string rucksack3)
		{
			Console.WriteLine($"Rucksack group: {rucksack1} {rucksack2} {rucksack3}");

			var commonItem = rucksack1.First(c => rucksack2.Contains(c) && rucksack3.Contains(c));
			var commonItemScore = char.IsUpper(commonItem) ? commonItem - 'A' + 27 : commonItem - 'a' + 1;
			Console.WriteLine($"\tCommon item: {commonItem} ({commonItemScore})");

			return commonItemScore;
		}
	}
}