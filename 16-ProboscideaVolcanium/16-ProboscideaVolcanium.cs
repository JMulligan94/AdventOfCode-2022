using System.Collections.Generic;
using System.Xml;

namespace _16_ProboscideaVolcanium
{
	internal class Program
	{
		class VolcanoState
		{
			private static int s_id = 0;
			public int Id { get; set; }
			public Valve LocationA { get; set; }
			public Valve LocationB { get; set; } = null;
			public bool HasSecondHelper => LocationB != null;
			public List<string> Opened { get; set; } = new List<string>();
			public int MinutesLeft { get; set; }
			public int PressureReleased { get; set; }

			public int TravelTimeLeftA { get; set; }
			public int TravelTimeLeftB { get; set; }

			public VolcanoState()
			{
				Id = s_id++;
			}

			public string GetLocationString()
			{
				var locationAString = TravelTimeLeftA == 0 ? LocationA.Label : $"~{LocationA.Label}";
				var locationBString = HasSecondHelper ? (TravelTimeLeftB == 0 ? LocationB.Label : $"~{LocationB.Label}") : "";
				return HasSecondHelper
					? $"{locationAString},{locationBString}" : $"{LocationA.Label}";
			}

			public string ToString() => $"{Id}-{GetLocationString()}|" +
							$"{string.Join(',', Opened)}|" +
							$"{MinutesLeft}|{PressureReleased}";
		}

		class Valve
		{
			public string Label { get; set; }
			public int FlowRate { get; set; }
			public List<Valve> ConnectedTo { get; set; } = new List<Valve>();
			public Dictionary<string, int> DistanceTo { get; set; } = new Dictionary<string, int>();

			public string ToString() => $"{Label}:{FlowRate}" +
				$" | {string.Join(',', ConnectedTo.Select(v => v.Label))}";
		}

		static void Main(string[] args)
		{
			var lines = File.ReadAllLines(args[0]);
			var valves = new Dictionary<string, Valve>();
			foreach (var line in lines)
			{
				var tokens = line.Split(' ');
				var flowRateString = tokens[4].Split('=')[1];
				valves.Add(tokens[1], new Valve()
				{
					Label = tokens[1],
					FlowRate = int.Parse(flowRateString.Substring(0, flowRateString.Length - 1))
				});
			}

			foreach (var line in lines)
			{
				var currentValve = line.Split(' ')[1];
				var connectedValvesStr = line.Substring(line.IndexOf("valve") + 6).Trim();
				var connectedValves = connectedValvesStr.Split(", ");
				foreach (var connectedValve in connectedValves)
				{
					valves[currentValve].ConnectedTo.Add(valves[connectedValve]);
					valves[currentValve].DistanceTo.Add(connectedValve, 1);
				}
			}

			// Calculate distances between all nodes
			for (var i = 0; i < valves.Count; i++)
			{
				foreach (var valve in valves.Values)
				{
					foreach (var connectedValve in valve.ConnectedTo)
					{
						foreach (var neighboursNeighbourPair in connectedValve.DistanceTo)
						{
							if (neighboursNeighbourPair.Key == valve.Label)
								continue;

							var newDistance = neighboursNeighbourPair.Value + 1;
							var currentDistance = int.MaxValue;
							if (valve.DistanceTo.ContainsKey(neighboursNeighbourPair.Key))
							{
								currentDistance = valve.DistanceTo[neighboursNeighbourPair.Key];
							}

							if (newDistance < currentDistance)
							{
								if (!valve.DistanceTo.TryAdd(neighboursNeighbourPair.Key, newDistance))
									valve.DistanceTo[neighboursNeighbourPair.Key] = newDistance;
							}
						}
					}
				}
			}

			var startValve = valves["AA"];
			// Remove any useless valves
			foreach (var valvePair in valves)
			{
				if (valvePair.Value.FlowRate == 0)
					valves.Remove(valvePair.Key);
			}

			foreach (var valveDistancePair in startValve.DistanceTo)
			{
				if (!valves.ContainsKey(valveDistancePair.Key))
					startValve.DistanceTo.Remove(valveDistancePair.Key);
			}

			foreach (var valvePair in valves)
			{
				foreach (var valveDistancePair in valvePair.Value.DistanceTo)
				{
					if (!valves.ContainsKey(valveDistancePair.Key))
						valvePair.Value.DistanceTo.Remove(valveDistancePair.Key);
				}
			}

			// Rules:
			//  Open valve = 1 minute 
			//  Travel through tunnel = 1 minute

			var destinationSets = GetSubsetIndicesList(valves.Count);
			// PART ONE - One person with 30 mins
			// Start with 30 mins from 'AA' to release as much pressure as possible
			var onePersonPaths = GetPaths(new VolcanoState()
			{
				LocationA = startValve,
				Opened = new List<string>(),
				MinutesLeft = 30,
				PressureReleased = 0
			}, ref valves);

			Console.WriteLine("\n===Part One:===");
			Console.WriteLine($"Total pressure released with 1 person in 30 mins: {onePersonPaths.First().PressureReleased}");

			// PART TWO - Two people with 26 mins
			// Start with 26 mins from 'AA' (for both) to release as much pressure as possible
			// Create all permutations of 2 subsets of destinations and run as if only one person is turning each side on

			//var twoPeoplePaths = GetPathsWithTwo(new VolcanoState()
			//{
			//	LocationA = startValve,
			//	LocationB = startValve,
			//	Opened = new List<string>(),
			//	MinutesLeft = 26,
			//	PressureReleased = 0
			//}, ref valves);
			//Console.WriteLine("\n===Part Two:===");
			//Console.WriteLine($"Total pressure released with 2 people in 26 mins: {twoPeoplePaths.First().PressureReleased}");
		}

		static List<List<int>> GetSubsetIndicesList(int length)
		{
			var subsetIndices = new List<List<int>>();
			var upper = Math.Pow(2, length);
			for (UInt16 i = 0; i < upper; ++i)
			{
				var subset = new List<int>();
				for (var bitPlace = 0; bitPlace < length; ++bitPlace)
				{
					var bitMask = 1 << bitPlace;
					if ((i & bitMask) != 0)
					{
						subset.Add(bitPlace);
					}
				}
				subsetIndices.Add(subset);
			}
			return subsetIndices;
		}

		static List<VolcanoState> GetPaths(VolcanoState initialState, ref Dictionary<string, Valve> valves)
		{
			var statesToCheck = new List<VolcanoState>() { initialState };
			var finishedStates = new List<VolcanoState>();
			while (statesToCheck.Count > 0)
			{
				var currentState = statesToCheck.First();
				statesToCheck.RemoveAt(0);

				//Console.WriteLine($"Checking location: '{currentState.GetLocationString()}', Minutes left: {currentState.MinutesLeft}");

				var newPressure = currentState.PressureReleased;
				var newOpened = new List<string>(currentState.Opened);
				// At a valve, does it need turning on?
				if (currentState.LocationA.FlowRate != 0
					&& !currentState.Opened.Contains(currentState.LocationA.Label))
				{
					// Turn on valve
					newPressure += currentState.LocationA.FlowRate * currentState.MinutesLeft;
					newOpened.Add(currentState.LocationA.Label);
				}

				if (newOpened.Count == valves.Count(v => v.Value.FlowRate != 0))
				{
					// All valves are opened
					Console.WriteLine("All valves are opened");
					finishedStates.Add(new VolcanoState()
					{
						LocationA = currentState.LocationA,
						Opened = newOpened,
						PressureReleased = newPressure,
						MinutesLeft = currentState.MinutesLeft
					});
					continue;
				}

				if (string.Join(',', currentState.Opened) == "DD,BB,JJ,HH,EE")
					Console.WriteLine();

				var hasPotentialDestination = false;
				// From location work out which unopened valves can be travelled to
				foreach (var valvePair in currentState.LocationA.DistanceTo)
				{
					var potentialDest = valves[valvePair.Key];
					if (potentialDest.FlowRate == 0 || currentState.Opened.Contains(valvePair.Key))
						continue; // Valve already open or useless

					// Is there enough time remaining to turn this valve on?
					var minutesToTurnOn = valvePair.Value + 1;
					if (currentState.MinutesLeft - minutesToTurnOn < 0)
					{
						// No time left to turn on
						continue;
					}

					hasPotentialDestination = true;
					statesToCheck.Add(new VolcanoState()
					{
						LocationA = potentialDest,
						Opened = newOpened,
						PressureReleased = newPressure,
						MinutesLeft = currentState.MinutesLeft - minutesToTurnOn,
					});
				}

				if (!hasPotentialDestination)
				{
					// No more potential valves to open in the time remaining
					finishedStates.Add(new VolcanoState()
					{
						LocationA = currentState.LocationA,
						Opened = newOpened,
						PressureReleased = newPressure,
						MinutesLeft = currentState.MinutesLeft
					});
					continue;
				}
			}

			return finishedStates.OrderByDescending(state => state.PressureReleased).ToList();
		}


		static List<VolcanoState> GetPathsWithTwo(VolcanoState initialState, ref Dictionary<string, Valve> valves)
		{
			var valvesList = valves.Values.ToList();

			var statesToCheck = new List<VolcanoState>() { initialState };
			var finishedStates = new List<VolcanoState>();
			while (statesToCheck.Count > 0)
			{
				var currentState = statesToCheck.First();
				statesToCheck.RemoveAt(0);

				if (currentState.Id == 3729)
					Console.WriteLine();
				//Console.WriteLine($"Checking location: '{currentState.GetLocationString()}', Minutes left: {currentState.MinutesLeft}");

				var turnValveA = currentState.TravelTimeLeftA == 0;
				var turnValveB = currentState.TravelTimeLeftB == 0;
				var newPressure = currentState.PressureReleased;
				var newOpened = new List<string>(currentState.Opened);
				// Open any valves either are stopped at
				if (turnValveA && currentState.LocationA.FlowRate != 0 
					&& !newOpened.Contains(currentState.LocationA.Label))
				{
					// Open valve at location A
					newPressure += (currentState.LocationA.FlowRate * currentState.MinutesLeft);
					newOpened.Add(currentState.LocationA.Label);
				}
				if (turnValveB && currentState.LocationB.FlowRate != 0
					&& !newOpened.Contains(currentState.LocationB.Label))
				{
					// Open valve at location B
					newPressure += (currentState.LocationB.FlowRate * currentState.MinutesLeft);
					newOpened.Add(currentState.LocationB.Label);
				}

				if (newOpened.Count == valves.Count(v => v.Value.FlowRate != 0)
					|| currentState.MinutesLeft <= 0)
				{
					// All valves are opened
					//Console.WriteLine("All valves are opened");
					finishedStates.Add(new VolcanoState()
					{
						LocationA = currentState.LocationA,
						LocationB = currentState.LocationB,
						PressureReleased = newPressure,
						Opened = newOpened
					});
					continue;
				}

				if (string.Join(',', newOpened) == "DD,JJ" && currentState.GetLocationString() == "JJ,~HH")
				{
					Console.WriteLine();
				}

				if (string.Join(',', newOpened) == "DD,JJ,BB,HH" && currentState.GetLocationString() == "BB,HH")
				{
					Console.WriteLine();
				}
				if (string.Join(',', newOpened) == "DD,JJ,BB,HH,CC" && currentState.GetLocationString() == "CC,~EE")
				{
					Console.WriteLine();
				}
				if (string.Join(',', newOpened) == "DD,JJ,BB,HH,CC,EE" && currentState.GetLocationString() == "CC,EE")
				{
					Console.WriteLine();
				}

				if (turnValveA && turnValveB)
				{
					
					for (var indexA = 0; indexA < valvesList.Count - 1; ++indexA)
					{
						var newLocationA = valvesList[indexA];

						if (newLocationA == currentState.LocationA)
							continue;

						if (newOpened.Contains(newLocationA.Label))
							continue; // Valve already opened

						var distanceToValveA = currentState.LocationA.DistanceTo[newLocationA.Label];

						// Wouldn't make it to the valve in time
						if (distanceToValveA > currentState.MinutesLeft)
							continue;


						for (var indexB = indexA + 1; indexB < valvesList.Count; ++indexB)
						{
							var newLocationB = valvesList[indexB];
							if (newLocationB == currentState.LocationB)
								continue;

							if (newOpened.Contains(newLocationB.Label))
								continue; // Valve already opened

							var distanceToValveB = currentState.LocationB.DistanceTo[newLocationB.Label];
							
							// Wouldn't make it to the valve in time
							if (distanceToValveB > currentState.MinutesLeft)
								continue;

							// Create new state to iterate on 
							var newState = new VolcanoState()
							{
								LocationA = newLocationA,
								LocationB = newLocationB,
								PressureReleased = newPressure,
								Opened = newOpened
							};

							var timeForA = distanceToValveA + 1;
							var timeForB = distanceToValveB + 1;
							// Which one arrives first?
							if (timeForA == timeForB)
							{
								// Same time
								newState.MinutesLeft = currentState.MinutesLeft - timeForA;
							}
							else if (timeForA < timeForB)
							{
								// A arrives before B
								newState.MinutesLeft = currentState.MinutesLeft - timeForA;
								newState.TravelTimeLeftB = timeForB - timeForA;
							}
							else
							{
								// B arrives before A
								newState.MinutesLeft = currentState.MinutesLeft - timeForB;
								newState.TravelTimeLeftA = timeForA - timeForB;
							}

							if (newState.MinutesLeft >= 0)
								statesToCheck.Add(newState);
						}
					}
				}
				else if (turnValveA)
				{
					var foundDestination = false;
					foreach (var valvePairA in currentState.LocationA.DistanceTo)
					{
						// Wouldn't make it to the valve in time
						if (valvePairA.Value > currentState.MinutesLeft)
							continue;

						var newLocationA = valves[valvePairA.Key];
						if (newOpened.Contains(valvePairA.Key)
							|| currentState.LocationB.Label == newLocationA.Label)
							continue; // Valve already opened, is useless, or B is already travelling to


						foundDestination = true;
						
						// Create new state to iterate on 
						var newState = new VolcanoState()
						{
							LocationA = newLocationA,
							LocationB = currentState.LocationB,
							PressureReleased = newPressure,
							Opened = newOpened
						};

						var timeForA = valvePairA.Value + 1;
						// Which one arrives first?
						if (timeForA == currentState.TravelTimeLeftB)
						{
							// Same time
							newState.MinutesLeft = currentState.MinutesLeft - timeForA;
							newState.TravelTimeLeftA = 0;
							newState.TravelTimeLeftB = 0;
						}
						else if (timeForA < currentState.TravelTimeLeftB)
						{
							// A arrives before B
							newState.MinutesLeft = currentState.MinutesLeft - timeForA;
							newState.TravelTimeLeftB = currentState.TravelTimeLeftB - timeForA;
							newState.TravelTimeLeftA = 0;
						}
						else
						{
							// B arrives before A
							newState.MinutesLeft = currentState.MinutesLeft - currentState.TravelTimeLeftB;
							newState.TravelTimeLeftA = timeForA - currentState.TravelTimeLeftB;
							newState.TravelTimeLeftB = 0;
						}

						if (newState.MinutesLeft >= 0)
							statesToCheck.Add(newState);
					}

					if (!foundDestination && currentState.TravelTimeLeftB > 0)
					{
						// B still needs to come to rest
						statesToCheck.Add(new VolcanoState()
						{
							LocationA = currentState.LocationA,
							LocationB = currentState.LocationB,
							MinutesLeft = currentState.MinutesLeft - currentState.TravelTimeLeftB,
							TravelTimeLeftA = 0,
							TravelTimeLeftB = 0,
							PressureReleased = newPressure,
							Opened = newOpened
						});
					}
				}
				else if (turnValveB)
				{
					var foundDestination = false;
					foreach (var valvePairB in currentState.LocationB.DistanceTo)
					{
						// Wouldn't make it to the valve in time
						if (valvePairB.Value > currentState.MinutesLeft)
							continue;

						var newLocationB = valves[valvePairB.Key];
						if (newOpened.Contains(valvePairB.Key)
							|| currentState.LocationA.Label == newLocationB.Label)
							continue; // Valve already opened, is useless, or A is already travelling to

						foundDestination = true;
						
						// Create new state to iterate on 
						var newState = new VolcanoState()
						{
							LocationA = currentState.LocationA,
							LocationB = newLocationB,
							PressureReleased = newPressure,
							Opened = newOpened
						};

						var timeForB = valvePairB.Value + 1;
						// Which one arrives first?
						if (currentState.TravelTimeLeftA == timeForB)
						{
							// Same time
							newState.MinutesLeft = currentState.MinutesLeft - currentState.TravelTimeLeftA;
							newState.TravelTimeLeftA = 0;
							newState.TravelTimeLeftB = 0;
						}
						else if (currentState.TravelTimeLeftA < timeForB)
						{
							// A arrives before B
							newState.MinutesLeft = currentState.MinutesLeft - currentState.TravelTimeLeftA;
							newState.TravelTimeLeftA = 0;
							newState.TravelTimeLeftB = timeForB - currentState.TravelTimeLeftA;
						}
						else
						{
							// B arrives before A
							newState.MinutesLeft = currentState.MinutesLeft - timeForB;
							newState.TravelTimeLeftA = currentState.TravelTimeLeftA - timeForB;
							newState.TravelTimeLeftB = 0;
						}

						if (newState.MinutesLeft >= 0)
							statesToCheck.Add(newState);
					}
				
					if (!foundDestination && currentState.TravelTimeLeftA > 0)
					{
						// A still needs to come to rest
						statesToCheck.Add(new VolcanoState()
						{
							LocationA = currentState.LocationA,
							LocationB = currentState.LocationB,
							MinutesLeft = currentState.MinutesLeft - currentState.TravelTimeLeftA,
							TravelTimeLeftA = 0,
							TravelTimeLeftB = 0,
							PressureReleased = newPressure,
							Opened = newOpened
						});
					}
				}
				statesToCheck = statesToCheck.OrderByDescending(s => s.PressureReleased).ToList();
			}
			return finishedStates.OrderByDescending(state => state.PressureReleased).ToList();
		}
	}
}