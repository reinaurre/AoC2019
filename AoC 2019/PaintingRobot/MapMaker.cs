using System;
using System.Collections.Generic;
using System.Text;
using WireManagement;

namespace PaintingRobot
{
    public class MapMaker
    {
        public Node[,] Map;
        private Coordinate max;
        private Coordinate min;

        public MapMaker(List<Coordinate> coordinates, Node.Symbol defaultSymbol = Node.Symbol.Empty)
        {
            this.CalculateBounds(coordinates);
            this.Map = new Node[Math.Abs(this.max.X)+Math.Abs(this.min.X)+1,Math.Abs(this.max.Y)+Math.Abs(this.min.Y)+1];
            this.BuildMap(defaultSymbol);
        }

        public void PopulatePaintMap(Dictionary<Coordinate,Color> mapValues)
        {
            foreach (KeyValuePair<Coordinate, Color> kvp in mapValues)
            {
                switch (kvp.Value)
                {
                    case Color.Black: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Node.Symbol.Dot; break;
                    case Color.White: Map[kvp.Key.X + Math.Abs(min.X), kvp.Key.Y + Math.Abs(min.Y)].Marker = Node.Symbol.Box; break;
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

        private void BuildMap(Node.Symbol defaultSymbol)
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
            this.max = new Coordinate(int.MinValue, int.MinValue);
            this.min = new Coordinate(int.MaxValue, int.MaxValue);
            
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
        public enum Symbol
        {
            Empty = ' ',
            O = 'O',
            BigX = 'X',
            SmallX = '×',
            Box = '■',
            Dot = '.',
            Hash = '#'
        }
    }
}
