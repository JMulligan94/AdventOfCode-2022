using System.Drawing;

namespace _09_RopeBridge
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var headMovements = File.ReadAllLines(args[0]).ToList();

			var shortRope = new Point[2] { new Point(), new Point() };
			var longRope = new Point[10];
			for (var i = 0; i< 10; ++i)
				longRope[i] = new Point();

			Console.WriteLine($"\n\n=== 2-segment Rope: ===");
			var shortRopeHistory = MoveRopeHead(headMovements, ref shortRope);

			Console.WriteLine($"\n\n=== 10-segment Rope: ===");
			var longRopeHistory = MoveRopeHead(headMovements, ref longRope);

			Console.WriteLine($"\n\n=== Part One: ===");
			Console.WriteLine($"Number of positions visited by tail of rope (2 segments): {shortRopeHistory.Count}");

			Console.WriteLine($"\n\n=== Part Two: ===");
			Console.WriteLine($"Number of positions visited by tail of rope (10 segments): {longRopeHistory.Count}");
		}

		static List<string> MoveRopeHead(List<string> movements, ref Point[] rope)
		{
			var tailHistory = new List<string>();

			foreach (var movement in movements)
			{
				Console.WriteLine($"\nMovement: {movement}");
				var direction = movement.Split(' ')[0][0];
				var amount = int.Parse(movement.Split(' ')[1]);

				var xMovement = 0;
				var yMovement = 0;
				switch (direction)
				{
					case 'U':
					{
						yMovement++;
					}
					break;

					case 'D':
					{
						yMovement--;
					}
					break;

					case 'L':
					{
						xMovement--;
					}
					break;

					case 'R':
					{
						xMovement++;
					}
					break;
				}

				for (var i = 0; i < amount; ++i)
					MoveHead(xMovement, yMovement, ref rope, ref tailHistory);
				
				Console.WriteLine($"\n{PrintRope(ref rope)}\n");
			}
			return tailHistory;
		}

		static void MoveHead(int xMovement, int yMovement, 
			ref Point[] rope, ref List<string> tailHistory)
		{
			rope[0] = new Point(rope[0].X + xMovement, rope[0].Y + yMovement);

			// Tail can only be 1 space away from head at most
			Console.WriteLine($"\tStart constrict - {PrintRope(ref rope)}");

			for(int segmentIndex = 0; segmentIndex < rope.Length - 1; segmentIndex++)
			{
				var backSegment = rope[segmentIndex + 1];
				var frontSegment = rope[segmentIndex];
				var distance = new Point(frontSegment.X - backSegment.X,
					frontSegment.Y - backSegment.Y);

				if ((Math.Abs(distance.X) > 1 && Math.Abs(distance.Y) != 0)
					|| Math.Abs(distance.X) != 0 && Math.Abs(distance.Y) > 1)
				{
					// If back segment isn't on the same row or column AND not touching,
					//  it moves diagonally to keep up
					var movementX = 0;
					var movementY = 0;
					if (distance.X > 1)
					{
						movementX++;
						movementY = distance.Y > 0 ? 1 : -1;
					}
					else if (distance.X < -1)
					{
						movementX--;
						movementY = distance.Y > 0 ? 1 : -1;
					}
					else if (distance.Y > 1)
					{
						movementX = distance.X > 0 ? 1 : -1;
						movementY++;
					}
					else if (distance.Y < -1)
					{
						movementX = distance.X > 0 ? 1 : -1;
						movementY--;
					}

					rope[segmentIndex + 1].X += movementX;
					rope[segmentIndex + 1].Y += movementY;
					Console.WriteLine($"\t\t\t{segmentIndex + 1} needs moving X({movementX}) and Y({movementY})");
				}
				else
				{
					if (distance.X > 1)
					{
						// Move back segment right
						Console.WriteLine($"\t\t\t{segmentIndex + 1} needs moving right");
						rope[segmentIndex + 1].X++;
						rope[segmentIndex + 1].Y = frontSegment.Y;
					}
					else if (distance.X < -1)
					{
						// Move back segment left
						Console.WriteLine($"\t\t\t{segmentIndex + 1} needs moving left");
						rope[segmentIndex + 1].X--;
						rope[segmentIndex + 1].Y = frontSegment.Y;
					}
					else if (distance.Y > 1)
					{
						// Move back segment up
						Console.WriteLine($"\t\t\t{segmentIndex + 1} needs moving up");
						rope[segmentIndex + 1].Y++;
						rope[segmentIndex + 1].X = frontSegment.X;
					}
					else if (distance.Y < -1)
					{
						// Move back segment down
						Console.WriteLine($"\t\t\t{segmentIndex + 1} needs moving down");
						rope[segmentIndex + 1].Y--;
						rope[segmentIndex + 1].X = frontSegment.X;
					}
				}
			}

			var tailSegment = rope.Last();
			if (!tailHistory.Contains(tailSegment.ToString()))
			{
				Console.WriteLine($"\t\t\tNew position! {tailSegment}");
				tailHistory.Add(tailSegment.ToString());
			}

			Console.WriteLine($"\tEnd constrict - {PrintRope(ref rope)}");

		}

		static string PrintRope(ref Point[] rope)
		{
			var ropeString = "";
			for (var segmentIndex = 0; segmentIndex < rope.Length; ++segmentIndex)
			{
				var segment = rope[segmentIndex];
				var segmentString;
				if (segmentIndex == 0)
					segmentString = "H";
				else if (segmentIndex == rope.Length - 1)
					segmentString = "T";
				else
					segmentString = $"{segmentIndex}";

				segmentString += $":({segment.X},{segment.Y})";
				ropeString += $"{segmentString}, ";
			}
			return ropeString;
		}
	}
}