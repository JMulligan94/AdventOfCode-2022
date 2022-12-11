namespace _08_TreetopTreeHouse
{
	internal class Program
	{
		enum VisibleDirection 
		{ 
			Up = 1 << 0,
			Right = 1 << 1,
			Down = 1 << 2,
			Left = 1 << 3
		}

		static void Main(string[] args)
		{
			var trees = File.ReadAllLines(args[0]);
			var totalRows = trees.Length;
			var totalCols = trees.First().Length;
			var visibleTrees = new List<int>();

			for (var row = 0; row < totalRows; row++)
			{
				for (var col = 0; col < totalCols; col++)
				{
					var height = trees[row][col];
					Console.WriteLine($"[{row},{col}]: {height}");
					var visibleSides = 0;

					// Check left
					var visible = true;
					Console.WriteLine($"\tChecking left...");
					var checkCol = col - 1;
					while (checkCol >= 0)
					{
						if (trees[row][checkCol] >= height)
						{
							Console.WriteLine($"\t\tBLOCKED...");
							visible = false;
							break;
						}
						checkCol--;
					}
					if (visible)
					{
						Console.WriteLine($"\t\tVISIBLE...");
						visibleSides |= (int)VisibleDirection.Left;
					}

					// Check right
					visible = true;
					Console.WriteLine($"\tChecking right...");
					checkCol = col + 1;
					while (checkCol < trees[row].Length)
					{
						if (trees[row][checkCol] >= height)
						{
							Console.WriteLine($"\t\tBLOCKED...");
							visible = false;
							break;
						}
						checkCol++;
					}
					if (visible)
					{
						Console.WriteLine($"\t\tVISIBLE...");
						visibleSides |= (int)VisibleDirection.Right;
					}

					// Check up
					visible = true;
					Console.WriteLine($"\tChecking up...");
					var checkRow = row - 1;
					while (checkRow >= 0)
					{
						if (trees[checkRow][col] >= height)
						{
							Console.WriteLine($"\t\tBLOCKED...");
							visible = false;
							break;
						}
						checkRow--;
					}
					if (visible)
					{
						Console.WriteLine($"\t\tVISIBLE...");
						visibleSides |= (int)VisibleDirection.Up;
					}

					// Check down
					visible = true;
					Console.WriteLine($"\tChecking down...");
					checkRow = row + 1;
					while (checkRow < trees.Length)
					{
						if (trees[checkRow][col] >= height)
						{
							Console.WriteLine($"\t\tBLOCKED...");
							visible = false;
							break;
						}
						checkRow++;
					}
					if (visible)
					{
						Console.WriteLine($"\t\tVISIBLE...");
						visibleSides |= (int)VisibleDirection.Down;
					}

					Console.WriteLine($"Visible directions: {visibleSides}");
					visibleTrees.Add(visibleSides);
				}
			}

			// Calculate scenic scores for each tree
			Console.WriteLine($"\n\nCalculating scenic scores...");
			var highestScenicScore = 0;
			for (var row = 1; row < trees.Length - 1; row++)
			{
				for (var col = 1; col < trees[row].Length - 1; col++)
				{
					var index = (row * totalCols) + col;
					var visibleDirections = visibleTrees[index];
					var height = trees[row][col];

					var visibleFromTop = (visibleDirections & (int)VisibleDirection.Up) != 0;
					var visibleFromLeft = (visibleDirections & (int)VisibleDirection.Left) != 0;
					var visibleFromRight = (visibleDirections & (int)VisibleDirection.Right) != 0;
					var visibleFromBottom = (visibleDirections & (int)VisibleDirection.Down) != 0;
					Console.WriteLine($"[{row},{col}]: {height} - {visibleDirections} -> " +
						$"U:{visibleFromTop} D:{visibleFromBottom} " +
						$"R:{visibleFromRight} L:{visibleFromLeft}");

					// Check left
					Console.WriteLine($"\tChecking left...");
					var checkCol = col - 1;
					var leftScore = 1;
					if (visibleFromLeft)
					{
						leftScore = col;
					}
					else
					{
						while (checkCol >= 0)
						{
							if (trees[row][checkCol] >= height)
							{
								Console.WriteLine($"\t\tBLOCKED...");
								break;
							}
							leftScore++;
							checkCol--;
						}
					}
					Console.WriteLine($"\t\tLeft score: {leftScore}");

					// Check right
					Console.WriteLine($"\tChecking right...");
					checkCol = col + 1;
					var rightScore = 1;
					if (visibleFromRight)
					{
						rightScore = totalCols - col - 1;
					}
					else
					{
						while (checkCol < trees[row].Length)
						{
							if (trees[row][checkCol] >= height)
							{
								Console.WriteLine($"\t\tBLOCKED...");
								break;
							}
							rightScore++;
							checkCol++;
						}
					}
					Console.WriteLine($"\t\tRight score: {rightScore}");

					// Check up
					Console.WriteLine($"\tChecking up...");
					var checkRow = row - 1;
					var upScore = 1;
					if (visibleFromTop)
					{
						upScore = row;
					}
					else
					{
						while (checkRow >= 0)
						{
							if (trees[checkRow][col] >= height)
							{
								Console.WriteLine($"\t\tBLOCKED...");
								break;
							}
							upScore++;
							checkRow--;
						}
					}
					Console.WriteLine($"\t\tUp score: {upScore}");

					// Check down
					Console.WriteLine($"\tChecking down...");
					checkRow = row + 1;
					var downScore = 1;
					if (visibleFromBottom)
					{
						downScore = totalRows - row - 1;
					}
					else
					{
						while (checkRow < trees.Length)
						{
							if (trees[checkRow][col] >= height)
							{
								Console.WriteLine($"\t\tBLOCKED...");
								break;
							}
							downScore++;
							checkRow++;
						}
					}
					Console.WriteLine($"\t\tDown score: {downScore}");

					var scenicScore = leftScore * rightScore * upScore * downScore;
					Console.WriteLine($"\tScenic score: {scenicScore}");
					highestScenicScore = Math.Max(highestScenicScore, scenicScore);
				}
			}


			Console.WriteLine("\n\n=== Part One: ===");
			var visibleCount = visibleTrees.Count(t => t != 0);
			Console.WriteLine($"Number of visible trees: {visibleCount}");

			Console.WriteLine("\n\n=== Part Two: ===");
			Console.WriteLine($"Highest Scenic score: {highestScenicScore}");

		}
	}
}