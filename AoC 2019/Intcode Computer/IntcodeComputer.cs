using System;
using System.Linq;

namespace Intcode_Computer
{
    public class IntcodeComputer
    {
        public string ComputeIntcode(string input)
        {
            // convert to int array
            int[] intcodeArray = input.ConvertToIntArray();

            // compute!
            intcodeArray = this.Compute(intcodeArray);

            return intcodeArray.ConvertToIntcodeString();
        }

        public object ComputeIntcode(int[] input, bool convertToString = true)
        {
            input = this.Compute(input);

            if (convertToString)
            {
                return input.ConvertToIntcodeString();
            }
            return input;
        }

        private int[] Compute(int[] input)
        {
            if(input.Length < 4)
            {
                return input;
            }

            int instructionPointer = 0;
            int nounPointer = 1;
            int verbPointer = 2;
            int outPointer = 3;

            while(input[instructionPointer] != 99 && instructionPointer+3 < input.Length)
            {
                if(!CheckValid(input, instructionPointer, nounPointer, verbPointer, outPointer))
                {
                    return input;
                }

                // Addition
                if(input[instructionPointer] == 1)
                {
                    input[input[outPointer]] = input[input[nounPointer]] + input[input[verbPointer]];
                }
                // Multiplication
                else if(input[instructionPointer] == 2)
                {
                    input[input[outPointer]] = input[input[nounPointer]] * input[input[verbPointer]];
                }
                // Finished
                else if(input[instructionPointer] == 99)
                {
                    return input;
                }
                else
                {
                    throw new Exception($"{input[instructionPointer]} is not a valid operator!");
                }

                instructionPointer += 4;
                nounPointer += 4;
                verbPointer += 4;
                outPointer += 4;
            }

            return input;
        }

        private bool CheckValid(int[] input, int instructionPointer, int nounPointer, int verbPointer, int outPointer)
        {
            if (input[instructionPointer] >= input.Length)
            {
                Console.WriteLine($"ERROR: instruction value {input[instructionPointer]} is larger than input length {input.Length}!");
                return false;
            }

            if (input[nounPointer] >= input.Length)
            {
                Console.WriteLine($"ERROR: noun value {input[nounPointer]} is larger than input length {input.Length}!");
                return false;
            }

            if (input[verbPointer] >= input.Length)
            {
                Console.WriteLine($"ERROR: verb value {input[verbPointer]} is larger than input length {input.Length}!");
                return false;
            }

            if (input[outPointer] >= input.Length)
            {
                Console.WriteLine($"ERROR: outPointer value {input[outPointer]} is larger than input length {input.Length}!");
                return false;
            }

            return true;
        }
    }

    public static class ExtensionMethods
    {
        public static int[] ConvertToIntArray(this string input)
        {
            return input.Split(',').Select(int.Parse).ToArray();
        }

        public static string ConvertToIntcodeString(this int[] input)
        {
            string output = string.Empty;

            for(int i = 0; i < input.Length; i++)
            {
                output += (i == input.Length - 1) ? $"{input[i]}" : $"{input[i]},";
            }

            return output;
        }
    }
}
