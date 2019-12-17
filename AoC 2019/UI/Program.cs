using System;
using System.IO;
using Intcode_Computer;
using WireManagement;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Day 2 Part 1:");
            Day2Part1();

            Console.WriteLine();
            Console.WriteLine("Day 2 Part 2:");
            Day2Part2();

            Console.WriteLine();
            Console.WriteLine("Day 3 Part 1:");
            Day3Part1();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public static void Day2Part1()
        {
            IntcodeComputer IC = new IntcodeComputer();

            Console.WriteLine("Parsing Input...");
            string[] input = File.ReadAllLines("Day2/Input.txt");

            int counter = 0;
            foreach (string str in input)
            {
                // Part 1
                Console.WriteLine();
                Console.WriteLine($"Computing line {counter}:");

                Console.WriteLine("Intcode Input:");
                Console.WriteLine(str);

                string output = IC.ComputeIntcode(str);

                Console.WriteLine("Intcode Output:");
                Console.WriteLine(output);

                counter++;
            }
        }

        public static void Day2Part2()
        {
            IntcodeComputer IC = new IntcodeComputer();

            Console.WriteLine("Parsing Input...");
            string[] input = File.ReadAllLines("Day2/Input.txt");

            bool isFound = false;
            int counter = 0;
            foreach (string str in input)
            {
                if (!isFound)
                {
                    //// Target: 19690720
                    Console.WriteLine();
                    Console.WriteLine($"Computing line {counter}:");

                    // convert to int array
                    int[] intcodeArray = str.ConvertToIntArray();

                    for (int noun = 0; noun <= 99; noun++)
                    {
                        if (!isFound)
                        {
                            for (int verb = 0; verb <= 99; verb++)
                            {
                                if (!isFound)
                                {
                                    int[] testIntcode = (int[])intcodeArray.Clone();
                                    testIntcode[1] = noun;
                                    testIntcode[2] = verb;

                                    // Console.WriteLine($"Noun = {noun}. Verb = {verb}");
                                    testIntcode = IC.ComputeIntcode(testIntcode);

                                    if (testIntcode[0] == 19690720)
                                    {
                                        Console.WriteLine($"Found! Noun = {noun}. Verb = {verb}.");
                                        isFound = true;
                                    }
                                }
                            }
                        }
                    }
                }

                counter++;
            }
        }

        public static void Day3Part1()
        {
            IntersectionDistanceCalculator IDC = new IntersectionDistanceCalculator();

            Console.WriteLine("Parsing Input...");
            string[] wires = File.ReadAllLines("Day3/Input1A.txt");

            Console.WriteLine($"Minimum Manhattan Distance = {IDC.FindSmallestManhattanDistance(wires[0].ConvertToCommands(), wires[1].ConvertToCommands())}");
            //PrintGrid(IDC.WireGrid, IDC.maxXsize, IDC.maxYsize);
        }

        private static void PrintGrid(int[,] grid, int maxX, int maxY)
        {
            StreamWriter outputTxt = File.CreateText("Day3/GridOutput.txt");

            for(int i = 0; i < maxY; i++)
            {
                for(int j = 0; j < maxX; j++)
                {
                    outputTxt.Write(grid[j, i]);
                }
                outputTxt.WriteLine();
            }

            outputTxt.Close();
        }
    }
}
