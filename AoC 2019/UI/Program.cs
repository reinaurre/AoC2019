﻿using System;
using System.IO;
using CodeCracker;
using Intcode_Computer;
using OrbitalCalculator;
using WireManagement;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine();
            //Console.WriteLine("Day 2 Part 1:");
            //Day2Part1();

            //Console.WriteLine();
            //Console.WriteLine("Day 2 Part 2:");
            //Day2Part2();

            //Console.WriteLine();
            //Console.WriteLine("Day 3 Part 1:");
            //Day3Part1();

            //Console.WriteLine();
            //Console.WriteLine("Day 3 Part 2:");
            //Day3Part2();

            //Console.WriteLine();
            //Console.WriteLine("Day 4 Part 1:");
            //Day4Part1();

            //Console.WriteLine();
            //Console.WriteLine("Day 4 Part 2:");
            //Day4Part2();

            //Console.WriteLine();
            //Console.WriteLine("Day 5 Part 1:");
            //Day5Part1();

            //Console.WriteLine();
            //Console.WriteLine("Day 5 Part 2:");
            //Day5Part2();

            //Console.WriteLine();
            //Console.WriteLine("Day 6 Part 1:");
            //Day6Part1();

            //Console.WriteLine();
            //Console.WriteLine("Day 6 Part 2:");
            //Day6Part2();

            //Console.WriteLine();
            //Console.WriteLine("Day 7 Part 1:");
            //Day7Part1();

            Console.WriteLine();
            Console.WriteLine("Day 7 Part 2:");
            Day7Part2();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        public static void Day2Part1()
        {
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

                IntcodeComputer IC = new IntcodeComputer(str);

                string output = IC.ComputeIntcodeToString();

                Console.WriteLine("Intcode Output:");
                Console.WriteLine(output);

                counter++;
            }
        }

        public static void Day2Part2()
        {
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

                                    IntcodeComputer IC = new IntcodeComputer(testIntcode);

                                    // Console.WriteLine($"Noun = {noun}. Verb = {verb}");
                                    testIntcode = IC.ComputeIntcode();

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
            //string fileName = "Day3/Input1B_Test.txt";
            string fileName = "Day3/Input1A.txt";

            Console.WriteLine("Parsing Input...");
            string[] wires = File.ReadAllLines(fileName);

            Console.WriteLine($"Minimum Manhattan Distance = {IDC.FindSmallestManhattanDistance(wires[0].ConvertToCommands(), wires[1].ConvertToCommands())}");
            
            if (fileName.Contains("Test"))
            {
                PrintGrid(IDC.WireGrid, IDC.maxXsize, IDC.maxYsize);
            }
        }

        public static void Day3Part2()
        {
            IntersectionDistanceCalculator IDC = new IntersectionDistanceCalculator();
            //string fileName = "Day3/Input1B_Test.txt";
            string fileName = "Day3/Input1A.txt";

            Console.WriteLine("Parsing Input...");
            string[] wires = File.ReadAllLines(fileName);

            Console.WriteLine($"Minimum Intersection Distance = {IDC.FindShortestIntersectionPath(wires[0].ConvertToCommands(), wires[1].ConvertToCommands())}");

            if (fileName.Contains("Test"))
            {
                // DisplayGrid(IDC.WireGrid, IDC.maxXsize, IDC.maxYsize);
                PrintGrid(IDC.WireGrid, IDC.maxXsize, IDC.maxYsize);
            }
        }

        public static void Day4Part1()
        {
            BruteForcer BF = new BruteForcer();

            int start = 264793;
            int end = 803935;

            // Test Values
            BF.BruteForce(111111, 111111); // true
            BF.BruteForce(223450, 223450); // false - 5->0
            BF.BruteForce(123789, 123789); // false - no double

            Console.WriteLine($"Start Value: {start}.");
            Console.WriteLine($"End Value: {end}.");

            Console.WriteLine($"Number of Valid Codes = {BF.GetNumberOfValidCodes(start, end)}");
        }

        public static void Day4Part2()
        {
            BruteForcer BF = new BruteForcer();

            int start = 264793;
            int end = 803935;

            // Test Values
            BF.BruteForce(112233, 112233, true); // true
            BF.BruteForce(123444, 123444, true); // false - 444 triple
            BF.BruteForce(111122, 111122, true); // true - double 2

            Console.WriteLine($"Start Value: {start}.");
            Console.WriteLine($"End Value: {end}.");

            Console.WriteLine($"Number of Valid Codes = {BF.GetNumberOfValidCodes(start, end, true)}");
        }

        public static void Day5Part1()
        {
            string fileName = "Day5/OfficialInput.txt";
            // string fileName = "Day5/Part1Test.txt";

            Console.WriteLine("Parsing Input...");
            string[] lines = File.ReadAllLines(fileName);

            int counter = 0;
            foreach (string str in lines)
            {
                int inputVal = 1;
                if (fileName.Contains("Test"))
                {
                    Console.Write("Enter Input Value: ");
                    string inputStr = Console.ReadLine();
                    inputVal = Convert.ToInt32(inputStr);
                }
                IntcodeComputer IC = new IntcodeComputer(str);

                string output = IC.ComputeIntcodeToString(inputVal);

                Console.WriteLine("Intcode Output:");
                Console.WriteLine(output);

                counter++;
            }
        }

        public static void Day5Part2()
        {
            string fileName = "Day5/OfficialInput.txt";
            // string fileName = "Day5/Part2Test.txt";

            Console.WriteLine("Parsing Input...");
            string[] lines = File.ReadAllLines(fileName);

            int counter = 0;
            foreach (string str in lines)
            {
                int inputVal = 5;
                if (fileName.Contains("Test"))
                {
                    Console.Write("Enter Input Value: ");
                    string inputStr = Console.ReadLine();
                    inputVal = Convert.ToInt32(inputStr);
                }
                IntcodeComputer IC = new IntcodeComputer(str);

                string output = IC.ComputeIntcodeToString(inputVal);

                Console.WriteLine("Intcode Output:");
                Console.WriteLine(output);

                counter++;
            }
        }

        public static void Day6Part1()
        {
            OrbitHierarchyCalculator OHC = new OrbitHierarchyCalculator();

            string fileName = "Day6/OfficialInput.txt";
            // string fileName = "Day6/Part1Test.txt";

            Console.WriteLine("Parsing Input...");
            string[] inputArr = File.ReadAllLines(fileName);

            Console.WriteLine($"Total direct & indirect orbits: {OHC.SumAllOrbitRelationships(inputArr)}");
        }

        public static void Day6Part2()
        {
            OrbitHierarchyCalculator OHC = new OrbitHierarchyCalculator();

            string fileName = "Day6/OfficialInput.txt";
            //string fileName = "Day6/Part2Test.txt";

            Console.WriteLine("Parsing Input...");
            string[] inputArr = File.ReadAllLines(fileName);

            Console.WriteLine($"Shortest transfers required: {OHC.FindShortestValidPath(inputArr)}");
        }

        public static void Day7Part1()
        {
            string fileName = "Day7/OfficialInput.txt";
            //string fileName = "Day7/Part1Test.txt"; // Test output should be 43210, 54321, then 65210

            Console.WriteLine("Parsing Input...");
            string[] lines = File.ReadAllLines(fileName);

            int counter = 0;
            foreach (string str in lines)
            {
                int maxvalue = 0;

                // HAHAHAHA THIS IS FUCKING GROSS
                for(int a = 0; a < 5; a++)
                {
                    for(int b = 0; b < 5; b++)
                    {
                        if(b != a)
                        {
                            for(int c = 0; c < 5; c++)
                            {
                                if(c != a && c != b)
                                {
                                    for(int d = 0; d < 5; d++)
                                    {
                                        if(d != a && d != b && d != c)
                                        {
                                            for(int e = 0; e < 5; e++)
                                            {
                                                if(e != a && e != b && e != c && e != d)
                                                {
                                                    IntcodeComputer IC1 = new IntcodeComputer(str);
                                                    IntcodeComputer IC2 = new IntcodeComputer(str);
                                                    IntcodeComputer IC3 = new IntcodeComputer(str);
                                                    IntcodeComputer IC4 = new IntcodeComputer(str);
                                                    IntcodeComputer IC5 = new IntcodeComputer(str);

                                                    int[] inputVals = new int[2] { a, 0 };
                                                    IC1.ComputeIntcode(inputVals);
                                                    inputVals[1] = IC1.OutputValue;

                                                    inputVals[0] = b;
                                                    IC2.ComputeIntcode(inputVals);
                                                    inputVals[1] = IC2.OutputValue;

                                                    inputVals[0] = c;
                                                    IC3.ComputeIntcode(inputVals);
                                                    inputVals[1] = IC3.OutputValue;

                                                    inputVals[0] = d;
                                                    IC4.ComputeIntcode(inputVals);
                                                    inputVals[1] = IC4.OutputValue;

                                                    inputVals[0] = e;
                                                    IC5.ComputeIntcode(inputVals);
                                                    inputVals[1] = IC5.OutputValue;

                                                    if (inputVals[1] > maxvalue)
                                                    {
                                                        maxvalue = inputVals[1];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Max Output:");
                Console.WriteLine(maxvalue);

                counter++;
            }
        }

        public static void Day7Part2()
        {
            string fileName = "Day7/OfficialInput.txt";
            //string fileName = "Day7/Part2Test.txt"; // Test output should be 139629729 seq: 9,8,7,6,5

            Console.WriteLine("Parsing Input...");
            string[] lines = File.ReadAllLines(fileName);

            int counter = 0;
            foreach (string str in lines)
            {
                int maxvalue = 0;

                // HAHAHAHA THIS IS FUCKING GROSS
                for (int a = 5; a <= 9; a++)
                {
                    for (int b = 5; b <= 9; b++)
                    {
                        if (b != a)
                        {
                            for (int c = 5; c <= 9; c++)
                            {
                                if (c != a && c != b)
                                {
                                    for (int d = 5; d <= 9; d++)
                                    {
                                        if (d != a && d != b && d != c)
                                        {
                                            for (int e = 5; e <= 9; e++)
                                            {
                                                if (e != a && e != b && e != c && e != d)
                                                {
                                                    int[] inputVals = new int[2] { a, 0 };

                                                    IntcodeComputer IC1 = new IntcodeComputer(str);
                                                    IntcodeComputer IC2 = new IntcodeComputer(str);
                                                    IntcodeComputer IC3 = new IntcodeComputer(str);
                                                    IntcodeComputer IC4 = new IntcodeComputer(str);
                                                    IntcodeComputer IC5 = new IntcodeComputer(str);

                                                    while (IC5.LatestOpcode != Opcodes.Terminate)
                                                    {
                                                        inputVals[0] = a;
                                                        IC1.ComputeIntcode(inputVals, true);
                                                        inputVals[1] = IC1.OutputValue;

                                                        inputVals[0] = b;
                                                        IC2.ComputeIntcode(inputVals, true);
                                                        inputVals[1] = IC2.OutputValue;

                                                        inputVals[0] = c;
                                                        IC3.ComputeIntcode(inputVals, true);
                                                        inputVals[1] = IC3.OutputValue;

                                                        inputVals[0] = d;
                                                        IC4.ComputeIntcode(inputVals, true);
                                                        inputVals[1] = IC4.OutputValue;

                                                        inputVals[0] = e;
                                                        IC5.ComputeIntcode(inputVals, true);
                                                        inputVals[1] = IC5.OutputValue;
                                                    }

                                                    if (inputVals[1] > maxvalue)
                                                    {
                                                        maxvalue = inputVals[1];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Max Output:");
                Console.WriteLine(maxvalue);

                counter++;
            }
        }

        private static void DisplayGrid(int[,] grid, int maxX, int maxY)
        {
            for (int i = maxY - 1; i >= 0; i--)
            {
                for (int j = 0; j < maxX; j++)
                {
                    if (grid[j, i] == 0)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write($"{grid[j, i]} ");
                    }
                }
                Console.WriteLine();
            }
        }

        private static void PrintGrid(int[,] grid, int maxX, int maxY)
        {
            StreamWriter outputTxt = File.CreateText("Day3/GridOutput.txt");

            for(int i = 0; i < maxY; i++)
            {
                for(int j = 0; j < maxX; j++)
                {
                    if (grid[j, i] == 0)
                    {
                        outputTxt.Write(".");
                    }
                    else
                    {
                        outputTxt.Write($"{grid[j, i]}");
                    }
                }
                outputTxt.WriteLine();
            }

            outputTxt.Close();
        }
    }
}
