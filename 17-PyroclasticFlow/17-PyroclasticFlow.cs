
namespace _17_PyroclasticFlow
{
    internal class Program
    {
        enum Direction { Left, Right }

        class RockShape
        {
            public string[] ShapeLines;

            public int Width => ShapeLines[0].Length; 
            public int Height => ShapeLines.Length; 

            public RockShape(string[] shapeLines)
            {
                ShapeLines = shapeLines;
            }
        }
        
        class Rock
        {
            public bool IsResting = false;
            public RockShape Shape { get; private set; }
            
            public int Row { get; set; }
            public int Column { get; set; }
            
            public Rock(RockShape shape)
            {
                Shape = shape;
            }
        }

        class Chamber
        {
            private readonly List<Direction> m_jetPattern;
            private int m_jetIndex = 0;
            
            public int HighestRow { get; private set; }
            private List<string> m_lines = new List<string>();

            private Rock? m_activeRock = null;

            public Chamber(List<Direction> jetPattern)
            {
                m_jetPattern = jetPattern;
            }
            
            public void AddLine()
            {
                m_lines.Add(".......");    
            }

            public bool MoveRockSideways(Direction direction)
            {
                if (m_activeRock == null)
                    return false;
                
                // Check if it can move sideways 
                
                // Chamber bounds check
                if ((m_activeRock.Column == 0 && direction == Direction.Left) || 
                    (m_activeRock.Column == 7 - m_activeRock.Shape.Width && direction == Direction.Right))
                    return false;

                // Check for other rested rocks
                var shapeLines = m_activeRock.Shape.ShapeLines;
                for (var shapeRowIndex = 0; shapeRowIndex < shapeLines.Length; ++shapeRowIndex)
                {
                    var globalRow = m_activeRock.Row + shapeRowIndex;
                    var globalColumn = -1;
                    var shapeLine = shapeLines[shapeRowIndex];
                    if (direction == Direction.Left)
                    {
                        var leftMostBlock = shapeLine.IndexOf('#');
                        globalColumn = m_activeRock.Column + leftMostBlock - 1;
                    }
                    else
                    {
                        var rightMostBlock = shapeLine.LastIndexOf('#');
                        globalColumn = m_activeRock.Column + rightMostBlock + 1;
                    }

                    if (m_lines[globalRow][globalColumn] == '#')
                        return false;
                }

                return true;
            }
            
            public bool MoveRockDown()
            {
                if (m_activeRock == null)
                    return false;
                
                // Check if it can move down 
                
                // Chamber bounds check
                if (m_activeRock.Row == 0)
                    return false;

                // Check for other rested rocks
                var shapeLines = m_activeRock.Shape.ShapeLines;
                for (var shapeRowIndex = 0; shapeRowIndex < shapeLines.Length; ++shapeRowIndex)
                {
                    var shapeLine = shapeLines[shapeRowIndex];
                    var globalRow = m_activeRock.Row + shapeRowIndex - 1;
                    for (var shapeColIndex = 0; shapeColIndex < shapeLine.Length; ++shapeColIndex)
                    {
                        var globalColumn = m_activeRock.Column + shapeColIndex;
                        if (m_lines[globalRow][globalColumn] == '#')
                            return false;
                    }
                }

                return true;
            }

            public void EraseRock()
            {
                if (m_activeRock == null)
                    return;

                for (var shapeRow = 0; shapeRow < m_activeRock.Shape.Height; ++shapeRow)
                {
                    var globalRow = m_activeRock.Row + shapeRow;
                    m_lines[globalRow] = m_lines[globalRow].Replace('@', '.');
                }
            }
            
            public void SetRockToRest()
            {
                if (m_activeRock == null)
                    return;

                m_activeRock.IsResting = true;

                HighestRow = Math.Max(HighestRow, m_activeRock.Row + m_activeRock.Shape.Height);

                for (var shapeRow = 0; shapeRow < m_activeRock.Shape.Height; ++shapeRow)
                {
                    var globalRow = m_activeRock.Row + shapeRow;
                    m_lines[globalRow] = m_lines[globalRow].Replace('@', '#');
                }

                m_activeRock = null;
            }

            public void PlaceRock(int row, int col)
            {
                if (m_activeRock == null)
                    return;

                m_activeRock.Row = row;
                m_activeRock.Column = col;
                
                var shapeLines = m_activeRock.Shape.ShapeLines;
                
                for (var shapeRow = 0; shapeRow < m_activeRock.Shape.Height; ++shapeRow)
                {
                    var globalRow = row + shapeRow;
                    var rowToPaste = shapeLines.Length - shapeRow - 1;
                    var shapeToPaste = shapeLines[rowToPaste].Replace('#', '@');
                    var modifiedLine = m_lines[globalRow].Remove(col, m_activeRock.Shape.Width);
                    modifiedLine = modifiedLine.Insert(col, shapeToPaste);
                    m_lines[globalRow] = modifiedLine;
                }
            }
            

            public void InitRock(Rock rock)
            {
                m_activeRock = rock;
                
                // Each rock appears so that:
                //  - its left edge is two units away from the left wall
                //  - its bottom edge is three units above the highest rock in the room (or the floor, if there isn't one).
                var col = 2;
                var row = HighestRow + 3;

                var currentLineCount = m_lines.Count;
                var shapeTopRow = row + m_activeRock.Shape.Height;
                if (shapeTopRow > currentLineCount)
                {
                    // Pad chamber with rows so we can place the rock
                    for (var i = 0; i < shapeTopRow - currentLineCount; ++i)
                    {
                        AddLine();
                    }
                }
                
                PlaceRock(row, col);
            }

            public void AdvanceTime()
            {
                if (m_activeRock == null)
                    return;
                
                var jetPush = m_jetPattern[m_jetIndex];
                m_jetIndex = (m_jetIndex + 1) % m_jetPattern.Count;
                
                // Pushed in jet direction
                if (MoveRockSideways(jetPush))
                {
                    //Console.WriteLine($"\nJet of gas pushes rock {jetPush}:");
                    EraseRock();
                    PlaceRock(m_activeRock.Row, m_activeRock.Column + (jetPush == Direction.Left ? -1 : 1));
                }
                else
                {
                    //Console.WriteLine($"\nJet of gas pushes rock {jetPush}, but nothing happens:");
                }
                
                //Print();
                
                // Fall down one unit
                if (MoveRockDown())
                {
                    //Console.WriteLine($"\nRock falls 1 unit");
                    EraseRock();
                    PlaceRock(m_activeRock.Row - 1, m_activeRock.Column);
                }
                else
                {
                    //Console.WriteLine($"\nRock falls 1 unit, causing it to come to rest");
                    
                    // Rock couldn't move further down, must be at rest
                    SetRockToRest();
                }
                //Print();
            }
            
            public void Print()
            {
                for(var lineIndex = m_lines.Count - 1; lineIndex >= 0; -- lineIndex)
                {
                    Console.WriteLine($"|{m_lines[lineIndex]}|");
                }    
                Console.WriteLine("+-------+");
            }
        }
        
        
        static void Main(string[] args)
        {
            var rockShapes = new List<RockShape>
            {
                new RockShape(new[] { "####" }),
                new RockShape(new[] { ".#.", "###", ".#." }),
                new RockShape(new[] { "..#", "..#", "###" }),
                new RockShape(new[] { "#", "#", "#", "#" }),
                new RockShape(new[] { "##", "##" }), 
            };
            
            var jetPattern = new List<Direction>();
            var jetPatternStr = File.ReadAllLines(args[0])[0];
            foreach (var jetPush in jetPatternStr)
            {
                jetPattern.Add(jetPush == '<' ? Direction.Left : Direction.Right);
            }
            
            var maxRocks = 20;

            var chamber = new Chamber(jetPattern);
            
            var rockShapeIndex = 0;
            for (var rockIndex = 0; rockIndex < maxRocks; ++rockIndex)
            {
                var currentRock = new Rock(rockShapes[rockShapeIndex]);
                rockShapeIndex = (rockShapeIndex + 1) % rockShapes.Count;
                
                Console.WriteLine($"\n{rockIndex})New rock begins falling:");
                chamber.InitRock(currentRock);
                chamber.Print();
                
                while (!currentRock.IsResting)
                {
                    chamber.AdvanceTime();
                }
                Console.WriteLine($"{rockIndex}) the tower is {chamber.HighestRow} units high");
            }
            

            Console.WriteLine("\n===Part One:===");
            //chamber.Print();
            Console.WriteLine($"After {maxRocks} rocks, the tower is {chamber.HighestRow} units high");
        }
    }
}