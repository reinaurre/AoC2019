using System;
using System.Collections.Generic;

namespace Intcode_Computer
{
    public class Operation
    {
        private readonly Dictionary<Opcodes, int> OperationLengths = new Dictionary<Opcodes, int>() {
            { Opcodes.Add, 4 },
            { Opcodes.Multiply, 4 },
            { Opcodes.Input, 2 },
            { Opcodes.Output, 2 },
            { Opcodes.JumpIfTrue, 3 },
            { Opcodes.JumpIfFalse, 3 },
            { Opcodes.LessThan, 4 },
            { Opcodes.Equals, 4 },
            { Opcodes.Terminate, 1 }
        };

        public Opcodes Opcode { get; private set; }
        public int Length;
        public bool Param1Immediate { get; private set; }
        public bool Param2Immediate { get; private set; }
        public bool Param3Immediate { get; private set; }

        public Operation(int instruction)
        {
            this.Param1Immediate = this.GetValueAtPosition(instruction, 100) == 1 ? true : false;
            this.Param2Immediate = this.GetValueAtPosition(instruction, 1000) == 1 ? true : false;
            this.Param3Immediate = this.GetValueAtPosition(instruction, 10000) == 1 ? true : false;

            int opcode = (this.GetValueAtPosition(instruction, 10) * 10) + this.GetValueAtPosition(instruction, 1);

            switch (opcode)
            {
                case (int)Opcodes.Add:
                    this.Opcode = Opcodes.Add;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.Multiply:
                    this.Opcode = Opcodes.Multiply;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.Input:
                    this.Opcode = Opcodes.Input;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.Output:
                    this.Opcode = Opcodes.Output;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.JumpIfTrue:
                    this.Opcode = Opcodes.JumpIfTrue;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.JumpIfFalse:
                    this.Opcode = Opcodes.JumpIfFalse;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.LessThan:
                    this.Opcode = Opcodes.LessThan;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.Equals:
                    this.Opcode = Opcodes.Equals;
                    this.Length = this.OperationLengths[this.Opcode];
                    break;
                case (int)Opcodes.Terminate:
                    this.Opcode = Opcodes.Terminate;
                    this.Length = 1;
                    break;
                default:
                    throw new ArgumentException($"ERROR: {opcode} is not a valid opcode!");
            }
        }

        private int GetValueAtPosition(int value, int position)
        {
            return (value % (position * 10) - value % position) / position;
        }
    }

    public enum Opcodes
    {
        Zero,
        Add = 1, // add the 1st parameter to the 2nd parameter and store the result in the position given by the 3rd parameter.
        Multiply = 2, // multiply the 1st parameter by the 2nd parameter and store the result in the position given by the 3rd parameter.
        Input = 3, // takes a single int as input and saves it to the position given by its only parameter.
        Output = 4, // outputs the value of its only parameter.
        JumpIfTrue = 5, // if the 1st parameter is non-zero, set the instruction pointer to the value from the 2nd parameter.
        JumpIfFalse = 6, // if the 1st parameter is zero, set the instruction pointer to the value from the 2nd parameter.
        LessThan = 7, // if the 1st parameter is less than the 2nd parameter, store 1 in in the position given by the 3rd parameter, otherwise store 0.
        Equals = 8, // if the 1st parameter is equal to the 2nd parameter, store 1 in the position given by the 3rd parameter, otherwise store 0.
        Terminate = 99
    }
}
