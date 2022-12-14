using System.Drawing;

namespace _12_HillClimbingAlgorithm
{
	internal class Program
	{
		enum Direction {  North = 1 << 0, East = 1 << 1, South = 1 << 2, West = 1 << 3 };

		public class Route
		{
			public List<Node> Path { get; set; } = new List<Node>();
			public int Length { get; set; }
		}

		public class Node
		{
			public int Row { get; set; }
			public int Col { get; set; }
			public char HeightChar { get; set; }
			public int Height { get; set; }
			public bool Visited { get; set; } // Has this node been visited?
			public int Score { get; set; } = int.MaxValue; // The score for the distance between this node and the start node so far

			public Node ScoreFrom { get; set; }

			public int ValidNeighbours { get; set; }
			public override string ToString()
			{
				return $"{Row},{Col}: {HeightChar}({Height})";
			}

			public void ResetNode()
			{
				Score = int.MaxValue;
				Visited = false;
				ScoreFrom = null;
			}
		}

		static void Main(string[] args)
		{
			var heightMap = File.ReadAllLines(args[0]);

			// Each letter = height with
			//   a lowest
			//   z highest

			var startIndex = 0;
			var endIndex = 0;

			var mapWidth = heightMap.First().Length;
			var mapHeight = heightMap.Length;
			var nodes = new Node[mapHeight * mapWidth];
			var startNodes = new List<int>();

			for (var row = 0; row < mapHeight; ++row)
			{
				for (var col = 0; col < mapWidth; ++col)
				{
					var index = (row * mapWidth) + col;
					var heightChar = heightMap[row][col];
					var height = heightChar - 'a';
					if (heightChar == 'S')
					{
						startIndex = index;
						startNodes.Insert(0, startIndex);
						height = -1;
					}
					else if (heightChar == 'E')
					{
						endIndex = index;
						height = 26;
					}

					var newNode = new Node()
					{
						Col = col,
						Row = row,
						Height = height,
						HeightChar = heightChar
					};

					if (row > 0)
					{
						var northHeight = heightMap[row - 1][col];
						if (IsNeighbourTraversible(height, northHeight))
							newNode.ValidNeighbours |= (int)Direction.North;
					}

					if (row < mapHeight - 1)
					{
						var southHeight = heightMap[row + 1][col];
						if (IsNeighbourTraversible(height, southHeight))
							newNode.ValidNeighbours |= (int)Direction.South;
					}

					if (col < mapWidth - 1)
					{
						var eastHeight = heightMap[row][col + 1];
						if (IsNeighbourTraversible(height, eastHeight))
							newNode.ValidNeighbours |= (int)Direction.East;
					}

					if (col > 0)
					{
						var westHeight = heightMap[row][col - 1];
						if (IsNeighbourTraversible(height, westHeight))
							newNode.ValidNeighbours |= (int)Direction.West;
					}

					nodes[index] = newNode;
					if (heightChar == 'a')
						startNodes.Add(index);
				}
			}

			// Part One - Start from 'S'
			var partOneRoute = FindShortestRoute(startIndex, endIndex, mapWidth, ref nodes);
			PrintRoute(partOneRoute);
			Console.WriteLine("\n\n=== Part One: ===");
			Console.WriteLine($"Shortest path from 'S' to 'E' takes {partOneRoute.Length} steps");

			// Part Two - Start from any 'a'
			RemoveExcessStartNodes(partOneRoute, mapWidth, ref startNodes);
			ResetNodes(ref nodes);

			var shortestRoute = new Route()
			{
				Length = int.MaxValue
			};

			foreach (var startNodeIndex in startNodes.Skip(1))
			{
				var route = FindShortestRoute(startNodeIndex, endIndex, mapWidth, ref nodes);
				if (route.Length < shortestRoute.Length)
					shortestRoute = route;

				RemoveExcessStartNodes(route, mapWidth, ref startNodes);
				ResetNodes(ref nodes);
			}

			Console.WriteLine("\n\n=== Part Two: ===");
			PrintRoute(shortestRoute);
			Console.WriteLine($"Shortest path from 'a' to 'E' takes {shortestRoute.Length} steps");
		}

		static void RemoveExcessStartNodes(Route route, int mapWidth, ref List<int> startNodes)
		{
			// Remove any but the final 'a's in the route,
			//  since these can't be any shorter when used as a starting point
			var finalAIndex = -1;
			var reversedRoute = route.Path;
			reversedRoute.Reverse();
			foreach(var routeNode in reversedRoute)
			{
				if (routeNode.HeightChar == 'a')
				{
					var index = GetTileArrayIndex(routeNode.Row, routeNode.Col, mapWidth);
					if (finalAIndex == -1)
					{
						finalAIndex = index;
						continue;
					}
					else if (startNodes.Contains(index))
					{
						Console.WriteLine($"Removing {routeNode} from start list, since it precedes another 'a' in a route");
						startNodes.Remove(index);
					}
				}
			}
			reversedRoute.Reverse();
		}

		static Route FindShortestRoute(int startIndex, int endIndex, int mapWidth, ref Node[] nodes)
		{
			var startNode = nodes[startIndex];
			startNode.Score = 0;

			var endNode = nodes[endIndex];
			var nodesToCheck = new List<Node>() { nodes[startIndex] };
			while (nodesToCheck.Count > 0)
			{
				var nodeToCheck = nodesToCheck.First();
				nodeToCheck.Visited = true;

				//Console.WriteLine($"Checking: {nodeToCheck}");

				// Found the end
				if (nodeToCheck == endNode)
					break;

				nodesToCheck.RemoveAt(0);

				if ((nodeToCheck.ValidNeighbours & (int)Direction.North) != 0)
				{
					var northNode = nodes[GetTileArrayIndex(nodeToCheck.Row - 1, nodeToCheck.Col, mapWidth)];

					CheckNeighbour(nodeToCheck, ref northNode);

					if (!northNode.Visited && !nodesToCheck.Contains(northNode))
						nodesToCheck.Add(northNode);
				}
				if ((nodeToCheck.ValidNeighbours & (int)Direction.South) != 0)
				{
					var southNode = nodes[GetTileArrayIndex(nodeToCheck.Row + 1, nodeToCheck.Col, mapWidth)];

					CheckNeighbour(nodeToCheck, ref southNode);

					if (!southNode.Visited && !nodesToCheck.Contains(southNode))
						nodesToCheck.Add(southNode);
				}

				if ((nodeToCheck.ValidNeighbours & (int)Direction.East) != 0)
				{
					var eastNode = nodes[GetTileArrayIndex(nodeToCheck.Row, nodeToCheck.Col + 1, mapWidth)];

					CheckNeighbour(nodeToCheck, ref eastNode);

					if (!eastNode.Visited && !nodesToCheck.Contains(eastNode))
						nodesToCheck.Add(eastNode);
				}
				if ((nodeToCheck.ValidNeighbours & (int)Direction.West) != 0)
				{
					var westNode = nodes[GetTileArrayIndex(nodeToCheck.Row, nodeToCheck.Col - 1, mapWidth)];

					CheckNeighbour(nodeToCheck, ref westNode);

					if (!westNode.Visited && !nodesToCheck.Contains(westNode))
						nodesToCheck.Add(westNode);
				}

				nodesToCheck.OrderByDescending(node => node.Score).ToList();
			}

			var path = new List<Node>();
			var node = endNode;
			while (node.ScoreFrom != null)
			{
				path.Insert(0, node);
				node = node.ScoreFrom;
			}
			path.Insert(0, node);

			return new Route()
			{
				Path = path,
				Length = endNode.Score
			};
		}

		static void ResetNodes(ref Node[] nodes)
		{
			foreach(var node in nodes)
			{
				node.ResetNode();
			}
		}

		static void PrintRoute(Route route)
		{
			Console.WriteLine("\n===Route from start to end===");
			foreach(var routeNode in route.Path)
			{
				Console.WriteLine(routeNode);
			}
		}

		static bool IsNeighbourTraversible(int current, int neighbour)
		{
			if (neighbour == 'S')
				return true;

			if (neighbour == 'E')
			{
				if (current == 25)
					return true;
			}
			else if ((neighbour - 'a') <= (current + 1))
				return true;

			return false;
		}

		static void CheckNeighbour(Node current, ref Node neighbour)
		{
			var newScore = current.Score + 1;
			if (neighbour.Score > newScore)
			{
				neighbour.Score = newScore;
				neighbour.ScoreFrom = current;
			}
		}
		
		static int GetTileArrayIndex(int row, int col, int mapWidth)
		{
			return row * mapWidth + col;
		}
	}
}