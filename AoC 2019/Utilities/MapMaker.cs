using System;
using System.Collections.Generic;

namespace Utilities
{
    public class MapMaker
    {
        public Node[,] Map;
        private Coordinate max;
        private Coordinate min;
        private bool hasScore = false;
        private long score = 0;

        public MapMaker(List<Coordinate> coordinates, Symbol defaultSymbol = Symbol.Empty)
        {
            this.CalculateBounds(coordinates);
            this.Map = new Node[Math.Abs(this.max.X)+Math.Abs(this.min.X)+1,Math.Abs(this.max.Y)+Math.Abs(this.min.Y)+1];
            this.BuildMap(defaultSymbol);
        }

        public void PopulateRepairMap(Dictionary<Coordinate,Symbol> mapValues)
        {
            foreach(KeyValuePair<Coordinate, Symbol> kvp in mapValues)
            {
                Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = kvp.Value;
            }
        }

        public void PopulatePaintMap(Dictionary<Coordinate,Color> mapValues)
        {
            foreach (KeyValuePair<Coordinate, Color> kvp in mapValues)
            {
                switch (kvp.Value)
                {
                    case Color.Black: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.Dot; break;
                    case Color.White: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.Box; break;
                }
            }
        }

        public void PopulateGameGrid(Dictionary<Coordinate, Tile> gridValues, long score)
        {
            if(score != 0)
            {
                this.hasScore = true;
                this.score = score;
            }

            foreach(KeyValuePair<Coordinate, Tile> kvp in gridValues)
            {
                switch (kvp.Value)
                {
                    case Tile.Empty: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.Empty; break;
                    case Tile.Ball: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.O; break;
                    case Tile.Block: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.Box; break;
                    case Tile.HPaddle: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.HLines; break;
                    case Tile.Wall: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Symbol.VLines; break;
                }
            }
        }

        public void PrintMap()
        {
            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                for (int x = 0; x < this.Map.GetLength(0); x++)
                {
                    Console.Write($"{(char)this.Map[x, y].Marker} ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Flickers a lot.  Recommend use PrintLiveUpdates for live view.
        /// </summary>
        /// <param name="addBuffer"></param>
        /// <param name="bufferSize"></param>
        public void PrintWholeMap(bool addBuffer = false, int bufferSize = 30)
        {
            string output = string.Empty;
            int outputRows = 0;

            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                for (int x = 0; x < this.Map.GetLength(0); x++)
                {
                    output += $"{(char)this.Map[x, y].Marker} ";
                }

                if (y == this.Map.GetLength(1) - 1 && this.hasScore)
                {
                    output += $"  Score: {this.score}";
                }

                output += "\n";
                outputRows++;
            }

            if (addBuffer)
            {
                string buffer = string.Empty;

                for (int i = 0; i < bufferSize - outputRows; i++)
                {
                    buffer += "\n";
                }

                output = buffer + output;
            }

            Console.Write(output);
        }

        public void PrintLiveUpdates()
        {
            Console.CursorVisible = false;

            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                Console.SetCursorPosition(10, y + 2);
                for (int x = 0; x < this.Map.GetLength(0); x++)
                {
                    Console.Write($"{(char)this.Map[x, y].Marker} ");
                }

                if (y == this.Map.GetLength(1) - 1 && this.hasScore)
                {
                    Console.Write($"  Score: {this.score}");
                }

                Console.WriteLine();
            }

            Console.CursorVisible = true;
        }

        private void BuildMap(Symbol defaultSymbol)
        {
            for (int y = 0; y < this.Map.GetLength(1); y++)
            {
                for (int x = 0; x < this.Map.GetLength(0); x++)
                {
                    Node current = new Node();
                    current.Coordinate = new Coordinate(x, y);
                    current.Marker = defaultSymbol;
                    Map[x, y] = current;
                }
            }
        }

        private void CalculateBounds(List<Coordinate> coordinates)
        {
            //this.max = new Coordinate(short.MinValue, short.MinValue);
            //this.min = new Coordinate(short.MaxValue, short.MaxValue);
            this.max = new Coordinate(0, 0);
            this.min = new Coordinate(0, 0);
            
            foreach(Coordinate coordinate in coordinates)
            {
                if(coordinate.X > max.X)
                {
                    max.SetX(coordinate.X);
                }
                else if(coordinate.X < min.X)
                {
                    min.SetX(coordinate.X);
                }

                if(coordinate.Y > max.Y)
                {
                    max.SetY(coordinate.Y);
                }
                else if(coordinate.Y < min.Y)
                {
                    min.SetY(coordinate.Y);
                }
            }
        }
    }

    public class Node
    {

        public Coordinate Coordinate;
        public Symbol Marker;
    }
}
