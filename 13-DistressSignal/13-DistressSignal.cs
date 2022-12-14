using System.IO.Pipes;
using System.Reflection.Metadata.Ecma335;

namespace _13_DistressSignal
{
	internal class Program
	{
		class PacketNode : IComparable<PacketNode>
		{
			public int? Value { get; set; }
			public PacketNode? Parent { get; set; }
			public List<PacketNode> Nodes { get; set; } = new List<PacketNode>();

			public bool IsEmpty => ToString() == "[]";

			public PacketNode(string input, PacketNode? parent)
			{
				Parent = parent;

				if (string.IsNullOrEmpty(input))
					return;

				if (input.First() == '[' && input.Last() == ']')
				{
					// Is List
					var newInput = input.Substring(1, input.Length - 2);
					var depth = 0;
					var tokens = new List<string>();
					var current = "";
					for (var i = 0; i < newInput.Length; i++)
					{
						if (newInput[i] == '[')
							depth++;
						else if (newInput[i] == ']')
							depth--;
						else if (newInput[i] == ',' && depth == 0)
						{
							tokens.Add(current);
							current = "";
							continue;
						}
						current += newInput[i];
					}
					tokens.Add(current);

					foreach (var token in tokens)
					{
						Nodes.Add(new PacketNode(token, this));
					}
				}
				else
				{
					// Value
					Value = int.Parse(input);
				}
			}

			public override string ToString()
			{
				if (!Nodes.Any())
					return Value.ToString();

				var nodesString = "[";
				foreach (var node in Nodes)
					nodesString += $"{node},";
				nodesString = $"{nodesString.TrimEnd(',')}]";

				return nodesString;
			}

			public int CompareTo(PacketNode? otherPacket)
			{
				if (otherPacket == null) 
					return 1;

				var pair = new PacketPair()
				{
					PacketA = this,
					PacketB = otherPacket
				};
				return pair.IsOrdered() ? -1 : 1;
			}
		}

		class PacketPair
		{
			public PacketNode PacketA { get; set; }
			public PacketNode PacketB { get; set; }

			public static PacketNode CreatePacketNode(string input)
			{
				return new PacketNode(input, null);
			}

			public bool IsOrdered()
			{
				var left = PacketA;
				var right = PacketB;
				var lowerFirst = false;
				
				CompareNodes(left, right, 0, ref lowerFirst);

				if (lowerFirst)
					Console.WriteLine("Left side is smaller, inputs are in the correct order");
				else
					Console.WriteLine("Right side is smaller, inputs are not in the correct order");

				return lowerFirst;
			}

			private bool CompareNodes(PacketNode left, PacketNode right, int depth, ref bool lowerFirst)
			{
				var padding = "";
				for (var i = 0; i < depth; i++)
					padding += " ";
				Console.WriteLine($"{padding}- Compare {left} vs {right}");

				if (left.Value != null && right.Value != null)
				{
					if (left.Value < right.Value)
					{
						lowerFirst = true;
						return false; // Stop recursing
					}
					else if (left.Value > right.Value)
					{
						lowerFirst = false;
						return false; // Stop recursing
					}
					return true;
				}
				else if (left.Nodes.Any() && right.Nodes.Any())
				{
					for (var nodeIndex = 0; nodeIndex < left.Nodes.Count; nodeIndex++)
					{
						if (right.Nodes.Count <= nodeIndex || (right.IsEmpty && !left.IsEmpty))
						{
							// Right ran out of items
							Console.WriteLine($"{padding}- Right side ran out of items; so inputs are not in the right order");
							lowerFirst = false;
							return false;
						}
						else
						{
							if (!CompareNodes(left.Nodes[nodeIndex], right.Nodes[nodeIndex], depth + 1, ref lowerFirst))
								return false;
						}
					}

					if (right.Nodes.Count > left.Nodes.Count || (left.IsEmpty && !right.IsEmpty))
					{
						// Left ran out of items
						Console.WriteLine($"{padding}- Left side ran out of items; so inputs are in the right order");
						lowerFirst = true;
						return false;
					}
				}
				else
				{
					if (left.Nodes.Any() && right.Value != null)
					{
						var convertedRight = CreatePacketNode($"[{right.Value}]");
						Console.WriteLine($"{padding} - Mixed types; convert right to {convertedRight} and retry");
						if (!CompareNodes(left, convertedRight, depth, ref lowerFirst))
							return false;
					}
					else if (right.Nodes.Any() && left.Value != null)
					{
						var convertedLeft = CreatePacketNode($"[{left.Value}]");
						Console.WriteLine($"{padding} - Mixed types; convert left to {convertedLeft} and retry");
						if (!CompareNodes(convertedLeft, right, depth, ref lowerFirst))
							return false;
					}
				}

				return true;
			}
			public override string ToString()
			{
				return $"A:{PacketA} B:{PacketB}";
			}
		}

		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);

			var pairs = new List<PacketPair>();
			for (var i = 0; i < lines.Length; i+=3) 
			{
				pairs.Add(new PacketPair()
				{
					PacketA = PacketPair.CreatePacketNode(lines[i]),
					PacketB = PacketPair.CreatePacketNode(lines[i+1])
				});
			}

			var numberOrderedPairs = new List<int>();
			for (var i = 0; i < pairs.Count; ++i)
			{
				Console.WriteLine($"\n== Pair {i+1} ==");
				if (pairs[i].IsOrdered())
				{
					numberOrderedPairs.Add(i+1);
				}
			}

			Console.WriteLine("\n\n=== Part One: ===");
			Console.WriteLine($"Sum of ordered pairs indices: {numberOrderedPairs.Sum()}");

			// Sort the pairs!
			var packets = new List<PacketNode>();
			pairs.ForEach(pair =>
			{
				packets.Add(pair.PacketA);
				packets.Add(pair.PacketB);
			});

			// Add [[2]] and [[6]]
			packets.Add(new PacketNode("[[2]]", null));
			packets.Add(new PacketNode("[[6]]", null));

			packets.Sort();

			var decoderIndexA = 0;
			var decoderIndexB = 0;
			for (var i = 0; i < packets.Count(); ++i)
			{
				if (packets[i].ToString() == "[[2]]")
					decoderIndexA = i + 1;
				else if (packets[i].ToString() == "[[6]]")
					decoderIndexB = i + 1;
			}

			Console.WriteLine("\n\n=== Part Two: ===");
			Console.WriteLine($"Decoder key for the distress signal is: {decoderIndexA * decoderIndexB}");
		}
	}
}