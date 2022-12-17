using System.Drawing;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;

namespace _15_BeaconExclusionZone
{
	class Sensor
	{
		public Point Position { get; set; }
		public Point Beacon { get; set; }
		public int DistanceToBeacon { get; set; }
	}
	internal class Program
	{
		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);
			var rowToTest = int.Parse(args[1]);
			var higherBound = int.Parse(args[2]);

			var sensors = new List<Sensor>();
			var beacons = new List<Point>();
			foreach (var line in lines)
			{
				var tokens = line.Split(' ');
				var sensorX = int.Parse(tokens[2].Substring(2, tokens[2].Length - 3));
				var sensorY = int.Parse(tokens[3].Substring(2, tokens[3].Length - 3));

				var beaconX = int.Parse(tokens[8].Substring(2, tokens[8].Length - 3));
				var beaconY = int.Parse(tokens[9].Substring(2));

				var closestBeacon = new Point(beaconX, beaconY);
				beacons.Add(closestBeacon);

				sensors.Add(new Sensor()
				{
					Position = new Point(sensorX, sensorY),
					Beacon = closestBeacon,
					DistanceToBeacon = Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY)
				});
			}

			var beaconsOnRow = 0;
			var coveredRanges = GetCoveredRangesInRow(rowToTest, sensors, out beaconsOnRow, true);

			var coveredPoints = 0;
			foreach(var range in coveredRanges)
			{
				coveredPoints += (range.Item2 - range.Item1) + 1;
			}
			coveredPoints -= beaconsOnRow;

			Console.WriteLine("\n=== Part One: ===");
			Console.WriteLine($"Number of places the beacon CANNOT be in row {rowToTest}: {coveredPoints}");

			Console.WriteLine("\n=== Part Two: ===");
			for(var row = 0; row < higherBound; ++row)
			{
				beaconsOnRow = 0;
				var ranges = GetCoveredRangesInRow(row, sensors, out beaconsOnRow, false);
				if (ranges.Count > 1 && beaconsOnRow == 0)
				{
					var hiddenBeaconX = ranges.First().Item2 + 1;
					Console.WriteLine($"Beacon should be at x={hiddenBeaconX} y={row}");

					ulong tuningFrequency = ((ulong)hiddenBeaconX * 4000000) + (uint)row;
					Console.WriteLine($"Tuning frequency: {tuningFrequency}");
				}
			}

		}

		static List<Tuple<int,int>> GetCoveredRangesInRow(int row, List<Sensor> sensors, out int beaconsOnRow, bool printDebug)
		{
			var coveredRanges = new List<Tuple<int, int>>();
			foreach (var sensor in sensors)
			{
				// Calc manhattan distance to row
				var distanceToRow = CalculateManhattanDistance(sensor.Position, new Point(sensor.Position.X, row));

				var overlap = sensor.DistanceToBeacon - distanceToRow;
				if (printDebug)
					Console.WriteLine($"Sensor: {sensor.Position} is {distanceToRow} away from row {row} (vs {sensor.DistanceToBeacon}) - overlap: {overlap}");
				if (overlap > 0)
				{
					if (printDebug)
						Console.WriteLine($"  Distance to row is less than the distance to the closest beacon by {overlap} units. This means these parts can't contain beacons here");

					coveredRanges.Add(new Tuple<int,int>(sensor.Position.X - overlap, sensor.Position.X + overlap));
				}
			}
			
			coveredRanges = coveredRanges.OrderBy(x => x.Item1).ToList();

			if (printDebug)
			{
				Console.WriteLine($"\nUnmerged ranges:");
				for (var rangeIndex = 0; rangeIndex < coveredRanges.Count; rangeIndex++)
				{
					Console.WriteLine($"  {coveredRanges[rangeIndex]}");
				}
			}

			// Join all overlapping covered ranges 
			if (printDebug)
				Console.WriteLine($"\nMerging ranges:");
			for (var rangeIndex = 0; rangeIndex < coveredRanges.Count - 1; rangeIndex++)
			{
				MergeWithOtherRanges(rangeIndex, ref coveredRanges, printDebug);
			}

			if (printDebug)
			{
				Console.WriteLine($"\nMerged ranges:");
				for (var rangeIndex = 0; rangeIndex < coveredRanges.Count; rangeIndex++)
				{
					Console.WriteLine($"  {coveredRanges[rangeIndex]}");
				}
			}

			beaconsOnRow = sensors.Select(s => s.Beacon).Where(b => b.Y == row).Distinct().Count();

			return coveredRanges;
		}

		static void MergeWithOtherRanges(int index, ref List<Tuple<int, int>> ranges, bool printDebug)
		{
			var rangesMerged = new List<int>(); 
			for (var rangeIndex = index + 1; rangeIndex < ranges.Count; ++rangeIndex)
			{
				var rangeB = ranges[rangeIndex];
				if (!RangesOverlap(ranges[index], rangeB))
					continue;

				if (printDebug)
					Console.WriteLine($"  {ranges[index]} and {rangeB} are overlapping:");
				// There is an overlap - merge ranges into one
				ranges[index] = new Tuple<int, int>(
					Math.Min(ranges[index].Item1, rangeB.Item1),
					Math.Max(ranges[index].Item2, rangeB.Item2));

				if (printDebug)
					Console.WriteLine($"    Merged into {ranges[index]}");

				rangesMerged.Add(rangeIndex);
			}

			// Remove any merged ones
			rangesMerged = rangesMerged.OrderByDescending(index => index).ToList();
			foreach(var mergedIndex in rangesMerged)
			{
				ranges.RemoveAt(mergedIndex);
			}
		}

		static bool RangesOverlap(Tuple<int, int> rangeA, Tuple<int, int> rangeB)
		{
			return !(rangeA.Item2 < rangeB.Item1 || rangeB.Item2 < rangeA.Item1);
		}

		static int CalculateManhattanDistance(Point a, Point b)
		{
			return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
		}
	}
}