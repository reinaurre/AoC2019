using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WireManagement
{
    public class IntersectionDistanceCalculator
    {
        public int[,] WireGrid { get; private set; }
        public int maxXsize;
        public int maxYsize;
        public Coordinate origin;

        private int Xmax = 0;
        private int Xmin = 0;
        private int Ymax = 0;
        private int Ymin = 0;

        private List<Intersection> intersectionList = new List<Intersection>();

        private const int WIRE_1_ID = 1; // self intersections = 2, joint = 4
        private const int WIRE_2_ID = 3; // self intersections = 6, joint = 4

        public int FindSmallestManhattanDistance(Command[] wire1, Command[] wire2)
        {
            this.BuildGraph(wire1, wire2);

            return this.intersectionList.SortByManhattanDistance().First(i => i.ManhattanDistance != 0).ManhattanDistance;
        }

        public int FindShortestIntersectionPath(Command[] wire1, Command[] wire2)
        {
            this.BuildGraph(wire1, wire2);

            List<Coordinate> path = new List<Coordinate>();
            path.Add(this.origin);
            this.FindShortestPath(this.origin.X, this.origin.Y, WIRE_1_ID, path);

            path = new List<Coordinate>();
            path.Add(this.origin);
            this.FindShortestPath(this.origin.X, this.origin.Y, WIRE_2_ID, path);

            int minPath = int.MaxValue;
            foreach(Intersection intsx in this.intersectionList)
            {
                int temp = this.CalculateIntersectionLength(intsx);
                if(temp != 0 && temp < minPath)
                {
                    minPath = temp;
                }
            }

            return minPath;
        }

        public int FindManhattanDistance(Intersection coordinate, int originX, int originY)
        {
            return coordinate.ManhattanDistance = Math.Abs((Math.Abs(coordinate.X) - Math.Abs(originX))) + Math.Abs((Math.Abs(coordinate.Y) - Math.Abs(originY)));
        }

        public int CalculateIntersectionLength(Intersection coordinate)
        {
            return coordinate.LengthA + coordinate.LengthB;
        }

        private void BuildGraph(Command[] wire1, Command[] wire2)
        {
            this.CalculateBoundaries(wire1);
            this.CalculateBoundaries(wire2);

            this.maxXsize = Math.Abs(this.Xmax) + Math.Abs(this.Xmin) + 1;
            this.maxYsize = Math.Abs(this.Ymax) + Math.Abs(this.Ymin) + 1;
            int startX = this.maxXsize - this.Xmax - 1;
            int startY = this.maxYsize - this.Ymax - 1;
            this.origin = new Coordinate(startX, startY);

            this.WireGrid = new int[this.maxXsize, this.maxYsize];
            this.PlotTrack(wire1, WIRE_1_ID);
            this.PlotTrack(wire2, WIRE_2_ID);

            for (int i = 0; i < intersectionList.Count; i++)
            {
                this.FindManhattanDistance(intersectionList[i], startX, startY);
            }
        }

        private void CalculateBoundaries(Command[] commands)
        {
            int curX = 0;
            int curY = 0;

            for(int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Direction)
                {
                    case Direction.Up:
                        curY += commands[i].Distance;

                        if (curY > this.Ymax)
                        {
                            this.Ymax = curY;
                        }
                        else if (curY < this.Ymin)
                        {
                            this.Ymin = curY;
                        }
                        break;
                    case Direction.Down:
                        curY -= commands[i].Distance;

                        if(curY > this.Ymax)
                        {
                            this.Ymax = curY;
                        }
                        else if(curY < this.Ymin)
                        {
                            this.Ymin = curY;
                        }
                        break;
                    case Direction.Left:
                        curX -= commands[i].Distance;

                        if (curX > this.Xmax)
                        {
                            this.Xmax = curX;
                        }
                        else if (curX < this.Xmin)
                        {
                            this.Xmin = curX;
                        }
                        break;
                    case Direction.Right:
                        curX += commands[i].Distance;

                        if(curX > this.Xmax)
                        {
                            this.Xmax = curX;
                        }
                        else if(curX < this.Xmin)
                        {
                            this.Xmin = curX;
                        }
                        break;
                }
            }
        }

        private void PlotTrack(Command[] commands, int trackNum)
        {
            int curX = this.origin.X;
            int curY = this.origin.Y;
            int pathLength = 0;

            for (int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Direction)
                {
                    case Direction.Up:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curY, ref pathLength);
                        break;
                    case Direction.Down:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curY, ref pathLength);
                        break;
                    case Direction.Left:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curX, ref pathLength);
                        break;
                    case Direction.Right:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curX, ref pathLength);
                        break;
                }
            }
        }

        // this can be moved elsewhere
        private void TraverseInstruction(ref int curX, ref int curY, int target, int trackNum, int delta, ref int axisValue, ref int pathLength)
        {
            for (int j = 0; j < target; j++)
            {
                if (this.WireGrid[curX, curY] == 0)
                {
                    this.WireGrid[curX, curY] = trackNum;
                }
                else if (this.WireGrid[curX, curY] != trackNum)
                {
                    this.WireGrid[curX, curY] += trackNum;
                    this.intersectionList.Add(new Intersection(curX, curY, this.WireGrid[curX,curY] == trackNum * 2));
                }

                axisValue += delta;
                pathLength++;
            }
        }

        // This should be moved elsewhere
        private void FindShortestPath(int curX, int curY, int trackNum, List<Coordinate> path)
        {
            // TODO: Stack Overflow
            if(this.intersectionList.Count(i => i.X == curX && i.Y == curY) > 0)
            {
                if(trackNum == WIRE_1_ID)
                {
                    this.intersectionList.First(i => i.X == curX && i.Y == curY).LengthA = path.Count;
                }
                else
                {
                    this.intersectionList.First(i => i.X == curX && i.Y == curY).LengthB = path.Count;
                }
            }
            else if(path.Count(p => p.X == curX && p.Y == curY) > 0)
            {
                return; // found a loop
            }

            path.Add(new Coordinate(curX, curY));

            if(curX > 0 && this.WireGrid[curX-1,curY] != 0)
            {
                this.FindShortestPath(curX - 1, curY, trackNum, path);
            }

            if (curX < this.Xmax && this.WireGrid[curX + 1, curY] != 0)
            {
                this.FindShortestPath(curX + 1, curY, trackNum, path);
            }

            if (curY < this.Ymax && this.WireGrid[curX, curY + 1] != 0)
            {
                this.FindShortestPath(curX, curY + 1, trackNum, path);
            }

            if (curY > 0 && this.WireGrid[curX, curY - 1] != 0)
            {
                this.FindShortestPath(curX, curY - 1, trackNum, path);
            }
        }
    }

    public static class ExtensionMethods
    {
        public static Command[] ConvertToCommands(this string input)
        {
            List<Command> output = new List<Command>();
            string[] inputArr = input.Split(',');

            for(int i = 0; i < inputArr.Length; i++)
            {
                if(inputArr[i].Length < 2)
                {
                    throw new ArgumentException($"ERROR: Step {inputArr[i]} is not valid!");
                }
                else if(!Regex.IsMatch(inputArr[i], "[u|d|l|r|U|D|L|R]{1}[0-9]+"))
                {
                    throw new ArgumentException($"ERROR: Step {inputArr[i]} is not valid!");
                }

                Direction direction;
                switch (inputArr[i][0])
                {
                    case 'U': direction = Direction.Up; break;
                    case 'D': direction = Direction.Down; break;
                    case 'L': direction = Direction.Left; break;
                    case 'R': direction = Direction.Right; break;
                    default: direction = Direction.Up; break; // it will never get here because of the regex above, but the compiler complains without it.
                }
                output.Add(new Command(direction, Convert.ToInt32(inputArr[i].Substring(1))));
            }

            return output.ToArray();
        }
    }
}
