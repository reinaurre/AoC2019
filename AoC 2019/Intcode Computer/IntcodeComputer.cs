using System;
using System.Linq;

namespace Intcode_Computer
{
    public class IntcodeComputer
    {
        private const string ACTION_NOUN = "noun";
        private const string ACTION_VERB = "verb";
        private const string ACTION_OPCODE = "opcode";
        private const string ACTION_OUTPOINTER = "outPointer";
        private const string ERROR_MESSAGE = "ERROR: {0} pointer value {1} is larger than input length {2}!";

        public string ComputeIntcode(string input)
        {
            // convert to int array
            int[] intcodeArray = input.ConvertToIntArray();

            // compute!
            intcodeArray = this.Compute(intcodeArray);

            return intcodeArray.ConvertToIntcodeString();
        }

        public int[] ComputeIntcode(int[] input)
        {
            return this.Compute(input);
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
                throw new ArgumentOutOfRangeException($"{ACTION_OPCODE} pointer", String.Format(ERROR_MESSAGE, ACTION_OPCODE, input[instructionPointer], input.Length));
            }

            if (input[nounPointer] >= input.Length)
            {
                throw new ArgumentOutOfRangeException($"{ACTION_NOUN} pointer", String.Format(ERROR_MESSAGE, ACTION_NOUN, input[nounPointer], input.Length));
            }

            if (input[verbPointer] >= input.Length)
            {
                throw new ArgumentOutOfRangeException($"{ACTION_VERB} pointer", String.Format(ERROR_MESSAGE, ACTION_VERB, input[verbPointer], input.Length));
            }

            if (input[outPointer] >= input.Length)
            {
                throw new ArgumentOutOfRangeException($"{ACTION_OUTPOINTER} pointer", String.Format(ERROR_MESSAGE, ACTION_OUTPOINTER, input[outPointer], input.Length));
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
