using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities;

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

            this.FindPathLengths(wire1, WIRE_1_ID);
            this.FindPathLengths(wire2, WIRE_2_ID);

            int minPath = int.MaxValue;
            foreach(Intersection intsx in this.intersectionList)
            {
                int temp = this.CalculateIntersectionLength(intsx);
                if(temp != 0 && temp < minPath && intsx.ManhattanDistance > 0)
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

            for (int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Direction)
                {
                    case Direction.Up:
                        this.PlotInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curY);
                        break;
                    case Direction.Down:
                        this.PlotInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curY);
                        break;
                    case Direction.Left:
                        this.PlotInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curX);
                        break;
                    case Direction.Right:
                        this.PlotInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curX);
                        break;
                }
            }
        }

        // this can be moved elsewhere
        private void PlotInstruction(ref int curX, ref int curY, int dest, int trackNum, int delta, ref int axisValue)
        {
            for (int j = 0; j < dest; j++)
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
            }
        }

        private void FindPathLengths(Command[] commands, int trackNum)
        {
            int curX = this.origin.X;
            int curY = this.origin.Y;
            int pathLength = 0;

            for(int i = 0; i < commands.Length; i++)
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

        private void TraverseInstruction(ref int curX, ref int curY, int dest, int trackNum, int delta, ref int axisValue, ref int pathLength)
        {
            for(int j = 0; j < dest; j++)
            {
                int x = curX;
                int y = curY;

                if (this.intersectionList.Count(i => i.X == x && i.Y == y) > 0)
                {
                    if (trackNum == WIRE_1_ID)
                    {
                        this.intersectionList.First(i => i.X == x && i.Y == y).LengthA = pathLength;
                    }
                    else
                    {
                        this.intersectionList.First(i => i.X == x && i.Y == y).LengthB = pathLength;
                    }
                }

                axisValue += delta;
                pathLength++;
            }
        }

        // This should be moved elsewhere
        private void FindShortestPath(int curX, int curY, int trackNum, List<Coordinate> path)
        {
            bool hasNext = true;
            int otherPathValue = trackNum == WIRE_1_ID ? WIRE_2_ID : WIRE_1_ID;

            while (hasNext)
            {
                if (this.intersectionList.Count(i => i.X == curX && i.Y == curY) > 0)
                {
                    if (trackNum == WIRE_1_ID)
                    {
                        // -1 because the root node is entered twice
                        this.intersectionList.First(i => i.X == curX && i.Y == curY).LengthA = path.Count-1;
                    }
                    else
                    {
                        this.intersectionList.First(i => i.X == curX && i.Y == curY).LengthB = path.Count-1;
                    }
                }
                else if (path.Count(p => p.X == curX && p.Y == curY) > 0)
                {
                    hasNext = false;
                    return; // found a loop
                }

                int pathOptions = 0;
                Direction dir = Direction.Up; // default doesn't matter, if it's used it'll have been assigned.

                int prevX = path[path.Count - 1].X;
                int prevY = path[path.Count - 1].Y;
                if (curX > 0 && this.WireGrid[curX - 1, curY] != 0 && this.WireGrid[curX-1,curY] != otherPathValue && curX - 1 != prevX)
                {
                    pathOptions++;
                    dir = Direction.Left;
                }
                if (curX < this.Xmax && this.WireGrid[curX + 1, curY] != 0 && this.WireGrid[curX + 1, curY] != otherPathValue && curX + 1 != prevX)
                {
                    pathOptions++;
                    dir = Direction.Right;
                }
                if (curY < this.Ymax && this.WireGrid[curX, curY + 1] != 0 && this.WireGrid[curX, curY + 1] != otherPathValue && curY + 1 != prevY)
                {
                    pathOptions++;
                    dir = Direction.Up;
                }
                if (curY > 0 && this.WireGrid[curX, curY - 1] != 0 && this.WireGrid[curX, curY - 1] != otherPathValue && curY - 1 != prevY)
                {
                    pathOptions++;
                    dir = Direction.Down;
                }

                path.Add(new Coordinate(curX, curY));

                if (pathOptions == 0)
                {
                    hasNext = false;
                    return;
                }
                else if(pathOptions == 1)
                {
                    switch (dir)
                    {
                        case Direction.Left: curX--; break;
                        case Direction.Right: curX++; break;
                        case Direction.Up: curY++; break;
                        case Direction.Down: curY--; break;
                    }
                }
                else
                {
                    if (curX > 0 && this.WireGrid[curX - 1, curY] != 0 && this.WireGrid[curX - 1, curY] != otherPathValue)
                    {
                        this.FindShortestPath(curX - 1, curY, trackNum, path);
                    }

                    if (curX < this.Xmax && this.WireGrid[curX + 1, curY] != 0 && this.WireGrid[curX + 1, curY] != otherPathValue)
                    {
                        this.FindShortestPath(curX + 1, curY, trackNum, path);
                    }

                    if (curY < this.Ymax && this.WireGrid[curX, curY + 1] != 0 && this.WireGrid[curX, curY + 1] != otherPathValue)
                    {
                        this.FindShortestPath(curX, curY + 1, trackNum, path);
                    }

                    if (curY > 0 && this.WireGrid[curX, curY - 1] != 0 && this.WireGrid[curX, curY - 1] != otherPathValue)
                    {
                        this.FindShortestPath(curX, curY - 1, trackNum, path);
                    }
                }
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
