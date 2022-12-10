using System.ComponentModel.Design.Serialization;
using System.IO;

namespace _07_NoSpaceLeftOnDevice
{
	internal class Program
	{

		class MockDirectory : MockFile
		{
			public Dictionary<string, MockFile> Files { get; set; } = new Dictionary<string, MockFile>();

			public override int CalculateSize()
			{
				return Files.Select(f => f.Value.CalculateSize()).Sum();
			}
		}

		class MockFile
		{
			public MockDirectory? ParentDirectory { get; set; }
			public string Path { get; set; }
			public int Size { get; set; }
			
			public virtual int CalculateSize()
			{
				return Size;
			}
		}

		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);
			
			var rootDirectory = new MockDirectory();
			var currentDirectory = rootDirectory;

			var lineNumber = 0;
			foreach (var line in lines.Where(l => l.StartsWith('$')))
			{
				Console.WriteLine($"{line}");
				var lineTokens = line.Split(' ');
				if (lineTokens[1] == "cd")
				{
					// Change Directory command
					if (lineTokens[2] == "/")
					{
						// Back to root
						currentDirectory = rootDirectory;
						Console.WriteLine($"\tMoved back to root");
					}
					else if (lineTokens[2] == "..")
					{
						// Up one level
						if (currentDirectory.ParentDirectory != null)
						{
							currentDirectory = currentDirectory.ParentDirectory;
							Console.WriteLine($"\tMoved up one level to /{string.Join("/", currentDirectory.Path)}");
						}
					}
					else
					{
						// Down one level
						if (!currentDirectory.Files.TryGetValue(lineTokens[2], out var dir))
						{
							dir = new MockDirectory()
							{
								Path = lineTokens[2],
								ParentDirectory = currentDirectory
							};
							currentDirectory.Files.Add(lineTokens[2], dir);
						}
						currentDirectory = dir as MockDirectory;
						Console.WriteLine($"\tMoved down one level to /{string.Join("/", currentDirectory.Path)}");
					}
					lineNumber++;
				}
				else if (lineTokens[1] == "ls")
				{
					Console.WriteLine($"\tListing contents of /{string.Join("/", currentDirectory.Path)}");
					// List contents of directory command
					// Iterate until next command 
					lineNumber++;
					while (lineNumber < lines.Count() && !lines[lineNumber].StartsWith('$'))
					{
						var listLineTokens = lines[lineNumber].Split(' ');
						Console.WriteLine($"\t\t {lines[lineNumber]}");
						if (listLineTokens[0] == "dir")
						{
							if (!currentDirectory.Files.TryGetValue(lineTokens[1], out var dir))
							{
								currentDirectory.Files.Add(listLineTokens[1], new MockDirectory()
								{
									Path = listLineTokens[1],
									ParentDirectory = currentDirectory
								});
							}
						}
						else
						{
							if (!currentDirectory.Files.TryGetValue(lineTokens[1], out var dir))
							{
								currentDirectory.Files.Add(listLineTokens[1], new MockFile()
								{
									Path = listLineTokens[1],
									Size = int.Parse(listLineTokens[0]),
									ParentDirectory = currentDirectory
								});
							}
						}
						lineNumber++;
					}
				}
			}

			Console.WriteLine("\n\n=== Part One: ===");
			var directories = GetFlatDirectoriesList(rootDirectory);
			var upperLimit = 100000;
			var partOneSum = 0;
			foreach (var directory in directories)
			{
				var size = directory.CalculateSize();
				if (size <= upperLimit)
					partOneSum += size;
				Console.WriteLine($"\tDir \'{directory.Path}\' (size={size})");
			}
			Console.WriteLine($"\nTotal size of files under 100,000: {partOneSum}");

			var totalSpace = 70000000;
			var updateSpace = 30000000;
			Console.WriteLine("\n\n=== Part Two: ===");
			Console.WriteLine($"\tTotal disk space: {totalSpace}");
			Console.WriteLine($"\tUpdate requires: {updateSpace}");

			var spaceUsed = rootDirectory.CalculateSize();
			var amountToDelete = spaceUsed - (totalSpace - updateSpace);
			Console.WriteLine($"\n\tSpace used: {spaceUsed}");
			Console.WriteLine($"\tAmount to delete: {amountToDelete}");
			var candidatesForDeletion = new List<int>();
			foreach (var directory in directories)
			{
				var size = directory.CalculateSize();
				if (size >= amountToDelete)
					candidatesForDeletion.Add(size);
				Console.WriteLine($"\t\tCould delete: Dir \'{directory.Path}\' (size={size})");
			}

			candidatesForDeletion = candidatesForDeletion.OrderBy(x => x).ToList();
			Console.WriteLine($"\nSmallest that can be deleted: {candidatesForDeletion.First()}");
		}

		static List<MockDirectory> GetFlatDirectoriesList(MockDirectory root)
		{
			var directories = new List<MockDirectory>();
			directories.Add(root);
			foreach (var directory in root.Files.Values.Where(f => f is MockDirectory).Cast<MockDirectory>())
			{
				directories.AddRange(GetFlatDirectoriesList(directory));
			}
			return directories;
		}
	}
}