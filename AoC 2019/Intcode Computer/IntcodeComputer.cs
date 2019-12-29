using System;
using System.Collections.Generic;
using System.Linq;

namespace Intcode_Computer
{
    public class IntcodeComputer
    {
        public Dictionary<long, long> Memory = new Dictionary<long, long>();
        public List<long> OutputValue = new List<long>();
        public Opcodes LatestOpcode;

        private long[] Intcode;
        private long Pointer = 0;
        private long InputPointer = 0;
        private long RelativeBase = 0;

        public IntcodeComputer(long[] intcode)
        {
            this.Intcode = intcode;
        }

        public IntcodeComputer(string intcodeStr)
        {
            this.Intcode = intcodeStr.ConvertToLongArray();
        }

        public void ModifyMemoryValue(long index, long value)
        {
            this.Intcode[index] = value;
        }

        public string ComputeIntcodeToString(long input = long.MaxValue)
        {
            long[] inputs = new long[1] { input };

            // compute!
            this.ExecuteOperations(inputs);

            return this.Intcode.ConvertToIntcodeString();
        }

        public string ComputeIntcodeToString(long[] inputVals)
        {
            // compute!
            this.ExecuteOperations(inputVals);

            return this.Intcode.ConvertToIntcodeString();
        }

        public long[] ComputeIntcode(long input = long.MaxValue)
        {
            long[] inputs = new long[1] { input };
            this.ExecuteOperations(inputs);
            return this.Intcode;
        }

        public long[] ComputeIntcode(long[] inputVals, bool stopOnOutput = false)
        {
            this.ExecuteOperations(inputVals, stopOnOutput);
            return this.Intcode;
        }

        public long[] ExecuteUntilOutputNumber(long[] inputs, int outputNumber)
        {
            this.OutputValue = new List<long>();

            for(int i = 0; i < outputNumber; i++)
            {
                this.ExecuteOperations(inputs, true);
            }

            return this.OutputValue.ToArray();
        }

        public long[] ExecuteOperations(long[] inputs, bool stopOnOutput = false)
        {
            this.LatestOpcode = Opcodes.Zero;
            this.InputPointer = 0;
            int counter = 0;

            //while(this.Pointer < this.Intcode.Length && this.LatestOpcode != Opcodes.Terminate)
            while(this.LatestOpcode != Opcodes.Terminate)
            {
                long operationOutput = this.InputPointer >= inputs.Length 
                    ? this.Compute() 
                    : this.Compute(inputs[this.InputPointer]);

                counter++;

                if (operationOutput != long.MinValue)
                {
                    this.OutputValue.Add(operationOutput);
                    
                    if (stopOnOutput)
                    {
                        return this.OutputValue.ToArray();
                    }
                }
            }

            return this.OutputValue.ToArray();
        }

        public long Compute(long input = long.MaxValue)
        {
            Operation op = new Operation(this.Intcode[this.Pointer]);
            this.LatestOpcode = op.Opcode;
            long output = long.MinValue;
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
                case Opcodes.AdjustBase:
                    this.AdjustBase(op);
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

        private void Add(Operation op)
        {
            long a = 0;
            long b = 0;
            long o = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "Add Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Add Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "Add Noun");
                    break;
            }

            switch (op.Param2Mode)
            {
                case Modes.Immediate: b = this.CheckValid(this.Intcode, this.Pointer + 2, "Add Verb"); break;
                case Modes.Position:
                    b = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Add Verb");
                    break;
                case Modes.Relative:
                    b = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "Add Verb");
                    break;
            }

            switch (op.Param3Mode)
            {
                case Modes.Immediate: o = this.Pointer + 3; break;
                case Modes.Position: o = this.CheckValid(this.Intcode, this.Pointer + 3, "Add Output"); break;
                case Modes.Relative: o = this.RelativeBase + this.Intcode[this.Pointer + 3]; break;
            }

            if (o >= this.Intcode.Length)
            {
                this.Memory[o] = a + b;
            }
            else
            {
                this.Intcode[o] = a + b;
            }
        }

        private void Multiply(Operation op)
        {
            long a = 0;
            long b = 0;
            long o = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "Multiply Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Multiply Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "Multiply Noun");
                    break;
            }

            switch (op.Param2Mode)
            {
                case Modes.Immediate: b = this.CheckValid(this.Intcode, this.Pointer + 2, "Multiply Verb"); break;
                case Modes.Position:
                    b = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Multiply Verb");
                    break;
                case Modes.Relative:
                    b = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "Multiply Verb");
                    break;
            }

            switch (op.Param3Mode)
            {
                case Modes.Immediate: o = this.Pointer + 3; break;
                case Modes.Position: o = this.CheckValid(this.Intcode, this.Pointer + 3, "Multiply Output"); break;
                case Modes.Relative: o = this.RelativeBase + this.Intcode[this.Pointer + 3]; break;
            }

            if (o >= this.Intcode.Length)
            {
                this.Memory[o] = a * b;
            }
            else
            {
                this.Intcode[o] = a * b;
            }
        }

        private void Input(long input, Operation op)
        {
            long dest = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: dest = this.Pointer + 1; break;
                case Modes.Position: dest = this.Intcode[this.Pointer + 1]; break;
                case Modes.Relative: dest = this.RelativeBase + this.Intcode[this.Pointer + 1]; break;
            }

            if (dest >= this.Intcode.Length)
            {
                this.Memory[dest] = input;
            }
            else
            {
                this.Intcode[dest] = input;
            }
        }

        private long Output(Operation op)
        {
            long source = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: source = this.Pointer + 1; break; // oops, no check for this one
                case Modes.Position: source = this.Intcode[this.Pointer + 1]; break;
                case Modes.Relative: source = this.RelativeBase + this.Intcode[this.Pointer + 1]; break;
            }

            if (source >= this.Intcode.Length)
            {
                return this.Memory[source];
            }
            else
            {
                return this.Intcode[source];
            }
        }

        private bool JumpIfTrue(Operation op)
        {
            long a = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "JumpIfTrue Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "JumpIfTrue Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "JumpIfTrue Noun");
                    break;
            }

            if(a != 0)
            {
                switch (op.Param2Mode)
                {
                    case Modes.Immediate: this.Pointer = this.CheckValid(this.Intcode, this.Pointer + 2, "JumpIfTrue Verb"); break;
                    case Modes.Position:
                        this.Pointer = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "JumpIfTrue Verb");
                        break;
                    case Modes.Relative:
                        this.Pointer = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "JumpIfTrue Verb");
                        break;
                }
                return true;
            }

            return false;
        }

        private bool JumpIfFalse(Operation op)
        {
            long a = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "JumpIfFalse Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "JumpIfFalse Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "JumpIfFalse Noun");
                    break;
            }

            if (a == 0)
            {
                switch (op.Param2Mode)
                {
                    case Modes.Immediate: this.Pointer = this.CheckValid(this.Intcode, this.Pointer + 2, "JumpIfFalse Verb"); break;
                    case Modes.Position:
                        this.Pointer = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "JumpIfFalse Verb");
                        break;
                    case Modes.Relative:
                        this.Pointer = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "JumpIfFalse Verb");
                        break;
                }
                return true;
            }

            return false;
        }

        private void LessThan(Operation op)
        {
            long a = 0;
            long b = 0;
            long o = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "LessThan Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "LessThan Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "LessThan Noun");
                    break;
            }

            switch (op.Param2Mode)
            {
                case Modes.Immediate: b = this.CheckValid(this.Intcode, this.Pointer + 2, "LessThan Verb"); break;
                case Modes.Position:
                    b = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "LessThan Verb");
                    break;
                case Modes.Relative:
                    b = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "LessThan Verb");
                    break;
            }

            switch (op.Param3Mode)
            {
                case Modes.Immediate: o = this.Pointer + 3; break; // oops, no check for this one
                case Modes.Position: o = this.CheckValid(this.Intcode, this.Pointer + 3, "LessThan Output"); break;
                case Modes.Relative: o = this.RelativeBase + this.Intcode[this.Pointer + 3]; break;
            }

            if (o >= this.Intcode.Length)
            {
                this.Memory[o] = a < b ? 1 : 0;
            }
            else
            {
                this.Intcode[o] = a < b ? 1 : 0;
            }
        }

        private void Equals(Operation op)
        {
            long a = 0;
            long b = 0;
            long o = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.CheckValid(this.Intcode, this.Pointer + 1, "Equals Noun"); break;
                case Modes.Position:
                    a = this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 1], "Equals Noun");
                    break;
                case Modes.Relative:
                    a = this.RelativeBase + this.Intcode[this.Pointer + 1] >= this.Intcode.Length
                        ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 1])
                        : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 1], "Equals Noun");
                    break;
            }

            switch (op.Param2Mode)
            {
                case Modes.Immediate: b = this.CheckValid(this.Intcode, this.Pointer + 2, "Equals Verb"); break;
                case Modes.Position:
                    b = this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.Intcode[this.Pointer + 2], "Equals Verb");
                    break;
                case Modes.Relative:
                    b = this.RelativeBase + this.Intcode[this.Pointer + 2] >= this.Intcode.Length
                            ? this.Memory.GetValueOrDefault(this.RelativeBase + this.Intcode[this.Pointer + 2])
                            : this.CheckValid(this.Intcode, this.RelativeBase + this.Intcode[this.Pointer + 2], "Equals Verb");
                    break;
            }

            switch (op.Param3Mode)
            {
                case Modes.Immediate: o = this.Pointer + 3; break; // oops, no check for this one
                case Modes.Position: o = this.CheckValid(this.Intcode, this.Pointer + 3, "Equals Output"); break;
                case Modes.Relative: o = this.RelativeBase + this.Intcode[this.Pointer + 3]; break;
            }

            if (o >= this.Intcode.Length)
            {
                this.Memory[o] = a == b ? 1 : 0;
            }
            else
            {
                this.Intcode[o] = a == b ? 1 : 0;
            }
        }

        private void AdjustBase(Operation op)
        {
            long a = 0;

            switch (op.Param1Mode)
            {
                case Modes.Immediate: a = this.Pointer + 1; break;
                case Modes.Position: a = this.CheckValid(this.Intcode, this.Pointer + 1, "Output Noun"); break;
                case Modes.Relative: a = this.RelativeBase + this.Intcode[this.Pointer + 1]; break;
            }

            if (a >= this.Intcode.Length)
            {
                this.RelativeBase += this.Memory[a];
            }
            else
            {
                this.RelativeBase += this.Intcode[a];
            }
        }

        private long CheckValid(long[] input, long pointer, string name)
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
        public static long[] ConvertToLongArray(this string input)
        {
            return input.Split(',').Select(long.Parse).ToArray();
        }

        public static string ConvertToIntcodeString(this long[] input)
        {
            string output = string.Empty;

            for(int i = 0; i < input.Length; i++)
            {
                output += (i == input.Length - 1) ? $"{input[i]}" : $"{input[i]},";
            }

            return output;
        }

        public static long GetValueOrDefault(this Dictionary<long,long> input, long key)
        {
            return input.ContainsKey(key) ? input[key] : 0;
        }
    }
}
