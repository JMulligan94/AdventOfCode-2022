using System.Globalization;

namespace _11_MonkeyInTheMiddle
{
	internal class Program
	{
		public delegate long MonkeyOperation(long old);

		class Monkey
		{
			public int Id { get; init; }
			public List<long> Items { get; init; }
			public MonkeyOperation Operation { get; init; }
			public long Test { get; init; }
			public int TrueMonkey { get; init; }
			public int FalseMonkey { get; init; }
		}

		static Monkey[] testMonkeys =
		{
			new Monkey
			{
				Id = 0,
				Items = new List<long> { 79, 98 },
				Operation = (old) => { return old * 19; },
				Test = 23,
				TrueMonkey = 2,
				FalseMonkey = 3,
			},
			new Monkey
			{
				Id = 1,
				Items = new List<long> { 54, 65, 75, 74 },
				Operation = (old) => { return old + 6; },
				Test = 19,
				TrueMonkey = 2,
				FalseMonkey = 0,
			},
			new Monkey
			{
				Id = 2,
				Items = new List<long> { 79, 60, 97 },
				Operation = (old) => { return old * old; },
				Test = 13,
				TrueMonkey = 1,
				FalseMonkey = 3,
			},
			new Monkey
			{
				Id = 3,
				Items = new List<long> { 74 },
				Operation = (old) => { return old + 3; },
				Test = 17,
				TrueMonkey = 0,
				FalseMonkey = 1
			}
		};

		static Monkey[] inputMonkeys =
		{
			new Monkey
			{
				Id = 0,
				Items = new List<long> { 73, 77 },
				Operation = (old) => { return old * 5; },
				Test = 11,
				TrueMonkey = 6,
				FalseMonkey = 5,
			},
			new Monkey
			{
				Id = 1,
				Items = new List<long> { 57, 88, 80 },
				Operation = (old) => { return old + 5; },
				Test = 19,
				TrueMonkey = 6,
				FalseMonkey = 0,
			},
			new Monkey
			{
				Id = 2,
				Items = new List<long> { 61, 81, 84, 69, 77, 88 },
				Operation = (old) => { return old * 19; },
				Test = 5,
				TrueMonkey = 3,
				FalseMonkey = 1,
			},
			new Monkey
			{
				Id = 3,
				Items = new List<long> { 78, 89, 71, 60, 81, 84, 87, 75 },
				Operation = (old) => { return old + 7; },
				Test = 3,
				TrueMonkey = 1,
				FalseMonkey = 0
			},
			new Monkey
			{
				Id = 4,
				Items = new List<long> { 60, 76, 90, 63, 86, 87, 89 },
				Operation = (old) => { return old + 2; },
				Test = 13,
				TrueMonkey = 2,
				FalseMonkey = 7
			},
			new Monkey
			{
				Id = 5,
				Items = new List<long> { 88 },
				Operation = (old) => { return old + 1; },
				Test = 17,
				TrueMonkey = 4,
				FalseMonkey = 7
			},
			new Monkey
			{
				Id = 6,
				Items = new List<long> { 84, 98, 78, 85 },
				Operation = (old) => { return old * old; },
				Test = 7,
				TrueMonkey = 5,
				FalseMonkey = 4
			},
			new Monkey
			{
				Id = 7,
				Items = new List<long> { 98, 89, 78, 73, 71 },
				Operation = (old) => { return old + 4; },
				Test = 2,
				TrueMonkey = 3,
				FalseMonkey = 2
			}
		};

		static void Main(string[] args)
		{
			var monkeys = new List<Monkey>();
			if (args[0] == "test.txt")
				monkeys = testMonkeys.ToList();
			else
				monkeys = inputMonkeys.ToList();

			var itemsHeld = new long[monkeys.Count()];

			var checkRounds = new int[] { 1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };

			var partToRun = 2;

			if (partToRun == 1)
			{
				for (var roundIndex = 0; roundIndex < 20; ++roundIndex)
				{
					Console.WriteLine($"\n===ROUND {roundIndex+1}===");
					for (var monkeyIndex = 0; monkeyIndex < monkeys.Count(); ++monkeyIndex)
					{
						var monkey = monkeys[monkeyIndex];
						Console.WriteLine($"Monkey {monkeyIndex}:");
						foreach (var item in monkey.Items)
						{
							Console.WriteLine($"  Monkey inspects an item with worry level {item}.");
							var itemScore = monkey.Operation(item);
							Console.WriteLine($"    Worry level is increased to {itemScore}.");

							itemScore = (int)Math.Floor(itemScore / 3.0);
							Console.WriteLine($"    Monkey gets bored. Worry level divided by 3 to {itemScore}.");

							var throwTo = 0;
							if (itemScore % monkey.Test == 0)
							{
								// Is divisible - pass
								Console.WriteLine($"    Worry level ~is~ divisible by {monkey.Test}.");
								throwTo = monkey.TrueMonkey;
							}
							else
							{
								// Not divisible - fail
								Console.WriteLine($"    Worry level not divisible by {monkey.Test}.");
								throwTo = monkey.FalseMonkey;
							}
							Console.WriteLine($"    Item {itemScore} thrown to monkey {throwTo}.");
							monkeys[throwTo].Items.Add(itemScore);
						}
						itemsHeld[monkeyIndex] += monkey.Items.Count;
						monkey.Items.Clear();
					}

					if (checkRounds.Contains(roundIndex + 1))
					{
						Console.WriteLine($"\n== After round {roundIndex + 1} ==");
						PrintMonkeyInspectionCounts(ref itemsHeld);
					}
				}

				Console.WriteLine("\n\n=== Part One: ===");
				PrintMonkeyInspectionCounts(ref itemsHeld);

				var orderedByActivity = itemsHeld.OrderByDescending(i => i).ToArray();
				var monkeyBusiness = orderedByActivity[0] * orderedByActivity[1];
				Console.WriteLine($"Monkey business score: {monkeyBusiness}");
			}
			else
			{ 
				long divisorProduct = 1;
				foreach (var monkey in monkeys)
					divisorProduct *= monkey.Test;

				for (var roundIndex = 0; roundIndex < 10000; ++roundIndex)
				{
					for (var monkeyIndex = 0; monkeyIndex < monkeys.Count(); ++monkeyIndex)
					{
						var monkey = monkeys[monkeyIndex];
						foreach (var item in monkey.Items)
						{
							var itemScore = monkey.Operation(item);
							itemScore = itemScore % divisorProduct;

							var throwTo = 0;
							if (itemScore % monkey.Test == 0)
							{
								// Is divisible - pass
								throwTo = monkey.TrueMonkey;
							}
							else
							{
								// Not divisible - fail
								throwTo = monkey.FalseMonkey;
							}
							monkeys[throwTo].Items.Add(itemScore);
						}
						itemsHeld[monkeyIndex] += monkey.Items.Count;
						monkey.Items.Clear();
					}

					if (checkRounds.Contains(roundIndex + 1))
					{
						Console.WriteLine($"\n== After round {roundIndex+1} ==");
						PrintMonkeyInspectionCounts(ref itemsHeld);
					}
				}
				Console.WriteLine("\n\n=== Part Two: ===");
				PrintMonkeyInspectionCounts(ref itemsHeld);

				var orderedByActivity = itemsHeld.OrderByDescending(i => i).ToArray();
				var monkeyBusiness = orderedByActivity[0] * orderedByActivity[1];
				Console.WriteLine($"Monkey business score: {monkeyBusiness}");
			}

		}

		static void PrintMonkeyInspectionCounts(ref long[] itemsHeld)
		{
			for (var monkeyIndex = 0; monkeyIndex < itemsHeld.Count(); ++monkeyIndex)
			{
				Console.WriteLine($"Monkey {monkeyIndex} inspected items {itemsHeld[monkeyIndex]} times.");
			}
		}
	}
}