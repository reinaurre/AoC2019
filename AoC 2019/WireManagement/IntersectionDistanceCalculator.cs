using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WireManagement
{
    public class IntersectionDistanceCalculator
    {
        public int[,] WireGrid { get; private set; }
        public int maxXsize;
        public int maxYsize;

        private int Xmax = 0;
        private int Xmin = 0;
        private int Ymax = 0;
        private int Ymin = 0;

        private List<Intersection> intersectionList = new List<Intersection>();

        public int FindSmallestManhattanDistance(Command[] wire1, Command[] wire2)
        {
            this.CalculateBoundaries(wire1);
            this.CalculateBoundaries(wire2);

            this.maxXsize = Math.Abs(this.Xmax) + Math.Abs(this.Xmin) + 1;
            this.maxYsize = Math.Abs(this.Ymax) + Math.Abs(this.Ymin) + 1;
            int startX = this.maxXsize - this.Xmax - 1;
            int startY = this.maxYsize - this.Ymax - 1;


            int wire1ID = 1;
            int wire2ID = 2;

            this.WireGrid = new int[this.maxXsize, this.maxYsize];
            this.PlotTrack(wire1, wire1ID, startX, startY);
            this.PlotTrack(wire2, wire2ID, startX, startY);

            int minValue = int.MaxValue;
            for(int i = 0; i < intersectionList.Count; i++)
            {
                int temp = this.FindManhattanDistance(intersectionList[i], startX, startY);
                if(temp < minValue && temp != 0)
                {
                    minValue = temp;
                }
            }

            return minValue;
        }

        public int FindManhattanDistance(Intersection coordinate, int originX, int originY)
        {
            return Math.Abs((Math.Abs(coordinate.X) - Math.Abs(originX))) + Math.Abs((Math.Abs(coordinate.Y) - Math.Abs(originY)));
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

        private void PlotTrack(Command[] commands, int trackNum, int startX, int startY)
        {
            int curX = startX;
            int curY = startY;

            for (int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Direction)
                {
                    case Direction.Up:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curY);
                        break;
                    case Direction.Down:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curY);
                        break;
                    case Direction.Left:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, -1, ref curX);
                        break;
                    case Direction.Right:
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Distance, trackNum, 1, ref curX);
                        break;
                }
            }
        }

        private void TraverseInstruction(ref int curX, ref int curY, int target, int trackNum, int delta, ref int axisValue)
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
                    this.intersectionList.Add(new Intersection(curX, curY));
                }
                axisValue += delta;
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
