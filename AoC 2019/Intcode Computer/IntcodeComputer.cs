using System;
using System.Collections.Generic;
using System.Linq;

namespace Intcode_Computer
{
    public class IntcodeComputer
    {
        public int OutputValue = int.MinValue;
        public Opcodes LatestOpcode;

        private int[] Intcode;
        private int Pointer = 0;
        private int InputPointer = 0;

        public IntcodeComputer(int[] intcode)
        {
            this.Intcode = intcode;
        }

        public IntcodeComputer(string intcodeStr)
        {
            this.Intcode = intcodeStr.ConvertToIntArray();
        }

        public string ComputeIntcodeToString(int input = int.MaxValue)
        {
            int[] inputs = new int[1] { input };

            // compute!
            this.ExecuteOperations(inputs);

            return this.Intcode.ConvertToIntcodeString();
        }

        public string ComputeIntcodeToString(int[] inputVals)
        {
            // compute!
            this.ExecuteOperations(inputVals);

            return this.Intcode.ConvertToIntcodeString();
        }

        public int[] ComputeIntcode(int input = int.MaxValue)
        {
            int[] inputs = new int[1] { input };
            this.ExecuteOperations(inputs);
            return this.Intcode;
        }

        public int[] ComputeIntcode(int[] inputVals, bool stopOnOutput = false)
        {
            this.ExecuteOperations(inputVals, stopOnOutput);
            return this.Intcode;
        }

        public int ExecuteOperations(int[] inputs, bool stopOnOutput = false)
        {
            this.LatestOpcode = Opcodes.Zero;
            this.InputPointer = this.Pointer == 0 ? 0 : 1;

            while(this.Pointer < this.Intcode.Length && this.LatestOpcode != Opcodes.Terminate)
            {
                int operationOutput = this.InputPointer >= inputs.Length 
                    ? this.Compute() 
                    : this.Compute(inputs[this.InputPointer]);

                if (operationOutput != int.MinValue && operationOutput != 0)
                {
                    this.OutputValue = operationOutput;
                    
                    if (stopOnOutput)
                    {
                        return this.OutputValue;
                    }
                }
            }

            return this.OutputValue;
        }

        public int Compute(int input = int.MaxValue)
        {
            Operation op = new Operation(this.Intcode[this.Pointer]);
            this.LatestOpcode = op.Opcode;
            int output = int.MinValue;
            bool skipIncrement = false;

            switch (op.Opcode)
            {
                case Opcodes.Add:
                    this.Add(op);
                    break;
                case Opcodes.Multiply:
                    this.Multiply(op);
                    break;
                case Opcodes.Input:
                    this.Input(input, op);
                    this.InputPointer++;
                    break;
                case Opcodes.Output:
                    output = this.Output(op);
                    break;
                case Opcodes.JumpIfTrue:
                    skipIncrement = this.JumpIfTrue(op);
                    break;
                case Opcodes.JumpIfFalse:
                    skipIncrement = this.JumpIfFalse(op);
                    break;
                case Opcodes.LessThan:
                    this.LessThan(op);
                    break;
                case Opcodes.Equals:
                    this.Equals(op);
                    break;
                case Opcodes.Terminate:
                    return output;
            }

            if (!skipIncrement)
            {
                this.Pointer += op.Length;
            }

            return output;
        }

        public void Reset()
        {

        }

        private void Add(Operation op)
        {
            int a = op.Param1Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "Add Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Add Noun");
            int b = op.Param2Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 2, "Add Verb")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Add Verb");
            int o = op.Param3Immediate
                ? this.Pointer + 3 // oops, no check for this one
                : this.CheckValid(this.Intcode, this.Pointer + 3, "Add Output");

            this.Intcode[o] = a + b;
        }

        private void Multiply(Operation op)
        {
            int a = op.Param1Immediate 
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "Multiply Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Multiply Noun");
            int b = op.Param2Immediate 
                ? this.CheckValid(this.Intcode, this.Pointer + 2, "Multiply Verb")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Multiply Verb");
            int o = op.Param3Immediate 
                ? this.Pointer + 3 // oops, no check for this one
                : this.CheckValid(this.Intcode, this.Pointer + 3, "Multiply Output");

            this.Intcode[o] = a * b;
        }

        private void Input(int input, Operation op)
        {
            int dest = op.Param1Immediate
                ? this.Pointer + 1
                : this.CheckValid(this.Intcode, this.Pointer + 1, "Input Noun");

            this.Intcode[dest] = input;
        }

        private int Output(Operation op)
        {
            int source = op.Param1Immediate
                ? this.Pointer + 1
                : this.CheckValid(this.Intcode, this.Pointer + 1, "Output Noun");

            return this.Intcode[source];
        }

        private bool JumpIfTrue(Operation op)
        {
            int a = op.Param1Immediate 
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "JumpIfTrue Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "JumpIfTrue Noun");

            if(a != 0)
            {
                this.Pointer = op.Param2Immediate 
                    ? this.CheckValid(this.Intcode, this.Pointer + 2, "JumpIfTrue Verb")
                    : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "JumpIfTrue Verb");
                return true;
            }

            return false;
        }

        private bool JumpIfFalse(Operation op)
        {
            int a = op.Param1Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "JumpIfFalse Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "JumpIfFalse Noun");

            if (a == 0)
            {
                this.Pointer = op.Param2Immediate
                    ? this.CheckValid(this.Intcode, this.Pointer + 2, "JumpIfFalse Verb")
                    : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "JumpIfFalse Verb");
                return true;
            }

            return false;
        }

        private void LessThan(Operation op)
        {
            int a = op.Param1Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "LessThan Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "LessThan Noun");
            int b = op.Param2Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 2, "LessThan Verb")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "LessThan Verb");
            int o = op.Param3Immediate
                ? this.Pointer + 3 // oops, no check for this one
                : this.CheckValid(this.Intcode, this.Pointer + 3, "LessThan Output");

            this.Intcode[o] = a < b ? 1 : 0;
        }

        private void Equals(Operation op)
        {
            int a = op.Param1Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 1, "Equals Noun")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Equals Noun");
            int b = op.Param2Immediate
                ? this.CheckValid(this.Intcode, this.Pointer + 2, "Equals Verb")
                : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Equals Verb");
            int o = op.Param3Immediate
                ? this.Pointer + 3 // oops, no check for this one
                : this.CheckValid(this.Intcode, this.Pointer + 3, "Equals Output");

            this.Intcode[o] = a == b ? 1 : 0;
        }

        private int CheckValid(int[] input, int pointer, string name)
        {
            if(pointer >= input.Length)
            {
                throw new ArgumentOutOfRangeException($"{name} pointer", $"ERROR: {name} pointer {pointer} is larger than array length {input.Length}!");
            }

            return input[pointer];
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
