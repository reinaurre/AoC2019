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

        private List<KeyValuePair<int, int>> intersectionList = new List<KeyValuePair<int, int>>();

        public int FindSmallestManhattanDistance(KeyValuePair<char, int>[] wire1, KeyValuePair<char, int>[] wire2)
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
            //this.FindIntersections(startX, startY, maxXsize, maxYsize, wire1ID + wire2ID);

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

        public int FindManhattanDistance(KeyValuePair<int,int> coordinate, int originX, int originY)
        {
            return Math.Abs((Math.Abs(coordinate.Key) - Math.Abs(originX))) + Math.Abs((Math.Abs(coordinate.Value) - Math.Abs(originY)));
        }

        private void CalculateBoundaries(KeyValuePair<char,int>[] commands)
        {
            int curX = 0;
            int curY = 0;

            for(int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Key)
                {
                    case 'U':
                        curY += commands[i].Value;

                        if (curY > this.Ymax)
                        {
                            this.Ymax = curY;
                        }
                        else if (curY < this.Ymin)
                        {
                            this.Ymin = curY;
                        }
                        break;
                    case 'D':
                        curY -= commands[i].Value;

                        if(curY > this.Ymax)
                        {
                            this.Ymax = curY;
                        }
                        else if(curY < this.Ymin)
                        {
                            this.Ymin = curY;
                        }
                        break;
                    case 'L':
                        curX -= commands[i].Value;

                        if (curX > this.Xmax)
                        {
                            this.Xmax = curX;
                        }
                        else if (curX < this.Xmin)
                        {
                            this.Xmin = curX;
                        }
                        break;
                    case 'R':
                        curX += commands[i].Value;

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

        private void PlotTrack(KeyValuePair<char, int>[] commands, int trackNum, int startX, int startY)
        {
            int curX = startX;
            int curY = startY;

            for (int i = 0; i < commands.Length; i++)
            {
                switch (commands[i].Key)
                {
                    case 'U':
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Value, trackNum, 1, ref curY);
                        break;
                    case 'D':
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Value, trackNum, -1, ref curY);
                        break;
                    case 'L':
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Value, trackNum, -1, ref curX);
                        break;
                    case 'R':
                        this.TraverseInstruction(ref curX, ref curY, commands[i].Value, trackNum, 1, ref curX);
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
                    this.intersectionList.Add(new KeyValuePair<int, int>(curX, curY));
                }
                axisValue += delta;
            }
        }
    }

    public static class ExtensionMethods
    {
        public static KeyValuePair<char,int>[] ConvertToCommands(this string input)
        {
            List<KeyValuePair<char, int>> output = new List<KeyValuePair<char, int>>();
            string[] inputArr = input.Split(',');

            Regex alpha = new Regex("[^a-zA-Z]");

            for(int i = 0; i < inputArr.Length; i++)
            {
                if(inputArr[i].Length < 2)
                {
                    throw new ArgumentException($"ERROR: Step {inputArr[i]} is not valid!");
                }
                else if(!Regex.IsMatch(inputArr[i], "[a-zA-Z]{1}[0-9]+"))
                {
                    throw new ArgumentException($"ERROR: Step {inputArr[i]} is not valid!");
                }

                output.Add(new KeyValuePair<char, int>(inputArr[i][0], Convert.ToInt32(inputArr[i].Substring(1))));
            }

            return output.ToArray();
        }
    }
}
