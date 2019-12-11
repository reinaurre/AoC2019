using System;
using System.IO;

namespace AoC_2019
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Reading Input");
            int[] inputValues = parseInput();

            Console.WriteLine("Total Fuel Requirement = " + calculateTotalFuelRequirement(inputValues));
            Console.ReadLine();
        }


        private static int[] parseInput()
        {
            string[] lines = File.ReadAllLines("Input.txt");
            int[] returnValues = new int[lines.Length];

            for(int i = 0; i < lines.Length; i++)
            {
                returnValues[i] = Convert.ToInt32(lines[i]);
            }

            return returnValues;
        }

        private static int calculateTotalFuelRequirement(int[] massValues)
        {
            int returnValue = 0;

            for(int i = 0; i < massValues.Length; i++)
            {
                returnValue += calculateFuelRequirement(massValues[i]);
            }

            return returnValue;
        }

        private static int calculateFuelRequirement(int massValue)
        {
            // divide by 3, round down, subtract 2

            int mass = Convert.ToInt32(Math.Floor(massValue / 3f) - 2);

            if(mass > 0)
            {
                return mass += calculateFuelRequirement(mass);
            }

            return 0;
        }
    }
}
