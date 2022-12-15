using System.Drawing;

namespace _14_RegolithReservoir
{
	internal class Program
	{
		class Map
		{
			public Point Offset { get; set; }

			public Point SandSpawn { get; set; }
			public List<Point> Rock { get; set; } = new List<Point>();
			public List<Point> Sand { get; set; } = new List<Point>();

			public Point MinExtents { get; set; }

			public Point MaxExtents { get; set; }

			public bool SpawnSand()
			{
				//Console.WriteLine("\nSpawning new sand!");
				var sand = new Point(SandSpawn.X, SandSpawn.Y);

				if (Sand.Contains(sand))
				{
					Console.WriteLine("\nSand spawn point is blocked!");
					return false;
				}

				var travelling = true;
				while(travelling)
				{
					if (sand.Y > MaxExtents.Y)
					{
						// Sand has fallen into the abyss!
						Console.WriteLine("\nSand is falling into the abyss! Stopping spawn");
						break;
					}

					var south = new Point(sand.X, sand.Y + 1);
					if (!Rock.Contains(south) && !Sand.Contains(south))
					{
						// Below is free, continue falling!
						sand = south;
					}
					else
					{
						// Below is blocked, try down and left first
						var southWest = new Point(sand.X - 1, sand.Y + 1);
						if (!Rock.Contains(southWest) && !Sand.Contains(southWest))
						{
							// Below is free, continue falling!
							sand = southWest;
						}
						else
						{
							// Down and left also blocked, try down and right!
							var southEast = new Point(sand.X + 1, sand.Y + 1);
							if (!Rock.Contains(southEast) && !Sand.Contains(southEast))
							{
								sand = southEast;
							}
							else
							{
								// All possible routes are blocked - sand has settled!
								travelling = false;
							}
						}
					}
				}

				if (!travelling)
					Sand.Add(sand);

				return !travelling; // While not falling into abyss, continue spawning sand
			}

			public void DrainSand()
			{
				Sand.Clear();
			}

			public void AddFloor(int extent)
			{
				for(var col = MinExtents.X - extent; col <= MaxExtents.X + extent; col++) 
				{
					Rock.Add(new Point(col, MaxExtents.Y + 2));
				}

				MinExtents = new Point(MinExtents.X - extent, MinExtents.Y);
				MaxExtents = new Point(MaxExtents.X + extent, MaxExtents.Y + 2);
			}

			public void PrintMap()
			{
				for (var y = MinExtents.Y; y <= MaxExtents.Y; ++y)
				{
					var rowStr = "";
					for (var x = MinExtents.X; x <= MaxExtents.X; ++x)
					{
						var current = new Point(x, y);
						if (current == SandSpawn)
							rowStr += '+';
						else if (Rock.Contains(current))
							rowStr += '#';
						else if (Sand.Contains(current))
							rowStr += 'o';
						else 
							rowStr += ".";
					}
					Console.WriteLine($"{y}\t{rowStr}");
				}
			}
		}

		static void Main(string[] args)
		{
			var rockInputs = File.ReadAllLines(args[0]);

			var rocks = new List<Point>();
			foreach (var rockInput in rockInputs)
			{
				var tokens = rockInput.Split(" -> ");
				var startPoint = new Point(
					int.Parse(tokens[0].Split(',')[0]),
					int.Parse(tokens[0].Split(",")[1]));
				
				rocks.Add(startPoint);

				foreach(var token in tokens.Skip(1)) 
				{
					var nextPoint = new Point(
						int.Parse(token.Split(',')[0]),
						int.Parse(token.Split(",")[1]));

					if (startPoint.X == nextPoint.X)
					{
						// Same column
						var startY = Math.Min(startPoint.Y, nextPoint.Y);
						var endY = Math.Max(startPoint.Y, nextPoint.Y);
						for (var y = startY; y <= endY; ++y)
						{
							var point = new Point(startPoint.X, y);
							if (!rocks.Contains(point))
								rocks.Add(point);
						}
					}
					else if (startPoint.Y == nextPoint.Y)
					{
						// Same row
						var startX = Math.Min(startPoint.X, nextPoint.X);
						var endX = Math.Max(startPoint.X, nextPoint.X);
						for (var x = startX; x < endX; ++x)
						{
							var point = new Point(x, startPoint.Y);
							if (!rocks.Contains(point))
								rocks.Add(point);
						}
					}

					if (!rocks.Contains(nextPoint))
						rocks.Add(nextPoint);
					startPoint = nextPoint;
				}
			}

			// Find extents of map
			var minX = Math.Min(500, rocks.MinBy(rock => rock.X).X);
			var minY = Math.Min(0, rocks.MinBy(rock => rock.Y).Y);
			var maxX = Math.Max(500, rocks.MaxBy(rock => rock.X).X);
			var maxY = Math.Max(0, rocks.MaxBy(rock => rock.Y).Y);

			var offset = new Point(minX, minY);
			var map = new Map();
			map.Offset = offset;
			map.MinExtents = new Point(minX - offset.X, minY - offset.Y);
			map.MaxExtents = new Point(maxX - offset.X, maxY - offset.Y);
			map.SandSpawn = new Point(500 - offset.X, 0 - offset.Y);
			map.Rock = rocks.Select(rock => new Point(rock.X - offset.X, rock.Y - offset.Y)).ToList();

			map.PrintMap();

			var totalSandHeld = 0;
			while (map.SpawnSand())
			{
				totalSandHeld++;
				//map.PrintMap();
			}

			map.PrintMap();

			Console.WriteLine("\n=== Part One: ===");
			Console.WriteLine($"Total sand held by structure: {totalSandHeld}");

			// Add floor
			map.DrainSand();

			map.PrintMap();

			map.AddFloor(500);

			map.PrintMap();

			var totalSandHeldBeforeBlock = 0;
			while (map.SpawnSand())
			{
				totalSandHeldBeforeBlock++;
			}

			map.PrintMap();

			Console.WriteLine("\n=== Part Two: ===");
			Console.WriteLine($"Total sand held by structure before blocking the spawn: {totalSandHeldBeforeBlock}");
		}
	}
}