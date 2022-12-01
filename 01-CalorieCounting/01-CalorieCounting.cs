using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _01_CalorieCounting
{
	class Program
	{
		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);
			var elfCalories = new List<int>();

			var runningCalorieTotal = 0;
			foreach(var line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					elfCalories.Add(runningCalorieTotal);
					runningCalorieTotal = 0;
					continue;
				}
				runningCalorieTotal += int.Parse(line);
			}
			elfCalories.Add(runningCalorieTotal);

			var elfCount = 1;
			foreach(var elf in elfCalories)
			{
				Console.WriteLine($"Elf {elfCount++}:\t{elf}");
			}

			Console.WriteLine("\n=== Part One ===");
			var sortedElfCalories = elfCalories.OrderByDescending(e => e).ToList();
			var maxCalories = sortedElfCalories.First();
			Console.WriteLine($"Max calories = {maxCalories}");


			Console.WriteLine("\n=== Part Two ===");
			var sumCalories = sortedElfCalories.Take(3).Sum();
			Console.WriteLine($"Sum of max 3 elf calories = {sumCalories}");
		}
	}
}
