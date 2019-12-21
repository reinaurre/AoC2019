using System;
using System.Collections.Generic;
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

        public string ComputeIntcode(string codeArr, int input = int.MaxValue)
        {
            // convert to int array
            int[] intcodeArray = codeArr.ConvertToIntArray();

            // compute!
            intcodeArray = this.Compute(intcodeArray, input);

            return intcodeArray.ConvertToIntcodeString();
        }

        public int[] ComputeIntcode(int[] codeArr, int input = int.MaxValue)
        {
            return this.Compute(codeArr, input);
        }

        private int[] Compute(int[] codeArr, int input)
        {
            int pointer = 0;
            List<int> output = new List<int>();

            while (pointer + 1 < codeArr.Length)
            {
                Operation op = new Operation(codeArr[pointer]);

                switch (op.Opcode)
                {
                    case Opcodes.Add:
                        this.Add(codeArr, pointer, op);
                        break;
                    case Opcodes.Multiply:
                        this.Multiply(codeArr, pointer, op);
                        break;
                    case Opcodes.Input:
                        this.Input(codeArr, pointer + 1, input, op);
                        break;
                    case Opcodes.Output:
                        output.Add(this.Output(codeArr, pointer + 1, op));
                        break;
                    case Opcodes.Terminate:
                        if (output.Count > 0)
                        {
                            return output.ToArray();
                        }
                        return codeArr;
                }

                pointer += op.Length;
            }

            if(output.Count > 0)
            {
                return output.ToArray();
            }

            return codeArr;
        }

        public void Add(int[] codeArr, int instructionPointer, Operation op)
        {
            int a = op.Param1Immediate ? codeArr[instructionPointer + 1] : codeArr[codeArr[instructionPointer + 1]];
            int b = op.Param2Immediate ? codeArr[instructionPointer + 2] : codeArr[codeArr[instructionPointer + 2]];
            int o = op.Param3Immediate ? instructionPointer + 3 : codeArr[instructionPointer + 3];

            codeArr[o] = a + b;
        }

        public void Multiply(int[] codeArr, int instructionPointer, Operation op)
        {
            int a = op.Param1Immediate ? codeArr[instructionPointer + 1] : codeArr[codeArr[instructionPointer + 1]];
            int b = op.Param2Immediate ? codeArr[instructionPointer + 2] : codeArr[codeArr[instructionPointer + 2]];
            int o = op.Param3Immediate ? instructionPointer + 3 : codeArr[instructionPointer + 3];

            codeArr[o] = a * b;
        }

        public void Input(int[] codeArr, int nounPointer, int input, Operation op)
        {
            int dest = op.Param1Immediate ? nounPointer : codeArr[nounPointer];

            codeArr[dest] = input;
        }

        public int Output(int[] codeArr, int nounPointer, Operation op)
        {
            int source = op.Param1Immediate ? nounPointer : codeArr[nounPointer];

            return codeArr[source];
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
