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
        Add = 1,
        Multiply = 2,
        Input = 3,
        Output = 4,
        Terminate = 99
    }
}
