using System;
using System.IO;
using CodeCracker;
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
            Console.WriteLine("Day 3 Part 2:");
            Day3Part2();

            Console.WriteLine();
            Console.WriteLine("Day 4 Part 1:");
            Day4Part1();

            Console.WriteLine();
            Console.WriteLine("Day 4 Part 2:");
            Day4Part2();

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
