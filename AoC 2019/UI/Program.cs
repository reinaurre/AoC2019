using System;
using System.IO;
using Intcode_Computer;

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

                string output = (string)IC.ComputeIntcode(str);

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

                                    Console.WriteLine($"Noun = {noun}. Verb = {verb}");
                                    testIntcode = (int[])IC.ComputeIntcode(testIntcode, false);

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
    }
}
