using System.Numerics;

namespace _02_RockPaperScissors
{
	class Program
	{
		public enum Choice
		{
			Rock, Paper, Scissors
		}
		public enum RoundOutcome
		{
			Lose, Draw, Win
		}

		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);

			var partOneScore = 0;
			var gameCount = 1;
			Console.WriteLine("\n=== Part One ===");
			foreach (var line in lines)
			{
				Console.WriteLine($"Game {gameCount++}: {line}");
				var opponent = (Choice)(line[0] - 'A');	// A(0) - Rock, B(1) - Paper, C(2) - Scissors
				var player = (Choice)(line[2] - 'X');     // X(0) - Rock, Y(1) - Paper, Z(2) - Scissors

				Console.WriteLine($"\t{opponent} vs {player}");

				var outcome = RoundOutcome.Draw;
				if (opponent == player)
				{
					// Draw - both chose the same
					Console.WriteLine("\t\tDraw!");
					outcome = RoundOutcome.Draw;
				}
				else
				{
					if (opponent == Choice.Scissors && player == Choice.Rock)
					{
						// Won against opponent
						Console.WriteLine("\t\tYou won!");
						outcome = RoundOutcome.Win;
					}
					else if (opponent == Choice.Rock && player == Choice.Scissors)
					{
						Console.WriteLine("\t\tYou lost!");
						outcome = RoundOutcome.Lose;
					}
					else if (opponent < player)
					{
						// Won against opponent
						Console.WriteLine("\t\tYou won!");
						outcome = RoundOutcome.Win;
					}
					else
					{
						Console.WriteLine("\t\tYou lost!");
						outcome = RoundOutcome.Lose;
					}
				}
				var playerScore = (int)player + 1;
				var outcomeScore = (int)outcome * 3;

				Console.WriteLine($"\tScore += {playerScore}({player}) + {outcomeScore}({outcome})");
				partOneScore += playerScore + outcomeScore;
			}
			Console.WriteLine($"\n\nFinal score when XYZ is player choice: {partOneScore}");


			Console.WriteLine("\n=== Part Two ===");
			gameCount = 1;
			var partTwoScore = 0;
			foreach (var line in lines)
			{
				Console.WriteLine($"\nGame {gameCount++}: {line}");
				var opponent = (Choice)(line[0] - 'A'); // A(0) - Rock, B(1) - Paper, C(2) - Scissors
				var outcome = (RoundOutcome)(line[2] - 'X');     // X(0) - Lose, Y(1) - Draw, Z(2) - Win

				Console.WriteLine($"\t{opponent} vs ?? = {outcome}");

				var roundScore = 3 * (int)outcome;
				var player = 0;
				switch (outcome)
				{
					case RoundOutcome.Draw:
					{
						Console.WriteLine("\t\tNeed draw...");
						player = (int)opponent; // Player chooses same as opponent
					}
					break;
					case RoundOutcome.Lose:
					{
						Console.WriteLine("\t\tNeed lose...");

						// Player chooses option BEFORE opponent (with wraparound for Rock)
						player = (int)opponent - 1;
						if (opponent == Choice.Rock)
							player = (int)Choice.Scissors;
					}
					break;
					case RoundOutcome.Win:
					{
						Console.WriteLine("\t\tNeed win...");

						// Player chooses option AFTER opponent (with wraparound for Scissors)
						player = (int)opponent + 1;
						if (opponent == Choice.Scissors)
							player = (int)Choice.Rock;
					}
					break;
				}
				var playerScore = player + 1;
				var outcomeScore = (int)outcome * 3;

				Console.WriteLine($"\tScore += {playerScore}({(Choice)player}) + {outcomeScore}({outcome})");
				partTwoScore += playerScore + outcomeScore;
			}

			Console.WriteLine($"\n\nFinal score when XYZ is round outcome: {partTwoScore}");
		}
	}
}