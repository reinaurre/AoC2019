using System;
using System.Collections.Generic;

namespace ChemicalReactor
{
    public class ChemicalReactor
    {
        private Dictionary<string, Reaction> Reactions;
        private Dictionary<string, int> Leftovers;

        private long OreAvailable = 0;

        public ChemicalReactor(string[] input)
        {
            this.Reactions = new Dictionary<string, Reaction>();
            this.ParseInputs(input);
        }

        public int CalculateMinimumRequired(string target, string initial, int amountRequired = 1)
        {
            this.Leftovers = new Dictionary<string, int>();

            Reaction temp;
            this.Reactions.TryGetValue(target, out temp);

            if(temp == null)
            {
                throw new Exception("Target reaction not found");
            }

            return this.DFS(temp, initial, amountRequired);
        }

        public long CalculateMaximumOutput(string target, string initial, long amountAvailable = 1000000000000)
        {
            this.Leftovers = new Dictionary<string, int>();
            this.OreAvailable = amountAvailable;

            Reaction temp;
            this.Reactions.TryGetValue(target, out temp);

            if (temp == null)
            {
                throw new Exception("Target reaction not found");
            }

            long cycles = 0;
            bool breakout = false;
            while (this.OreAvailable > 0)
            {
                this.DFS(temp, initial, 1);
                cycles++;

                bool clear = true;
                foreach(KeyValuePair<string, int> kvp in this.Leftovers)
                {
                    if(kvp.Value != 0)
                    {
                        clear = false;
                        break;
                    }
                }

                if (clear)
                {
                    breakout = true;
                    break;
                }
            }

            if (breakout)
            {
                long orePerLoop = amountAvailable - this.OreAvailable;
                long multiplier = amountAvailable / orePerLoop;
                this.OreAvailable = amountAvailable % orePerLoop;
                cycles *= multiplier;

                while(this.OreAvailable > 0)
                {
                    this.DFS(temp, initial, 1);
                    cycles++;
                }
            }

            cycles--;

            return cycles;
        }

        private int DFS(Reaction root, string initial, int multiplier)
        {
            if (root.Inputs.ContainsKey(initial))
            {
                int amount = root.Inputs[initial] * multiplier;
                this.OreAvailable -= amount;
                return amount;
            }

            int count = 0;

            foreach (KeyValuePair<string, int> kvp in root.Inputs)
            {
                Reaction temp;
                this.Reactions.TryGetValue(kvp.Key, out temp);

                if(temp == null)
                {
                    throw new Exception("No Reaction found");
                }

                int required = kvp.Value * multiplier;
                if(this.Leftovers.ContainsKey(kvp.Key))
                {
                    if(this.Leftovers[kvp.Key] >= required)
                    {
                        this.Leftovers[kvp.Key] -= required;
                        continue;
                    }
                    else
                    {
                        required -= this.Leftovers[kvp.Key];
                        this.Leftovers[kvp.Key] = 0;
                    }
                }

                int prodMultiplier = 0;
                if(temp.OutputQuantity > required)
                {
                    prodMultiplier = 1;
                }
                else
                {
                    prodMultiplier = (required + temp.OutputQuantity - 1) / temp.OutputQuantity;
                }

                if (this.Leftovers.ContainsKey(kvp.Key))
                {
                    this.Leftovers[kvp.Key] += (temp.OutputQuantity * prodMultiplier) - required;
                }
                else
                {
                    this.Leftovers.Add(kvp.Key, (temp.OutputQuantity * prodMultiplier) - required);
                }

                count += this.DFS(temp, initial, prodMultiplier);
            }

            return count;
        }

        private void ParseInputs(string[] input)
        {
            foreach(string str in input)
            {
                Reaction reaction = new Reaction(str);

                this.Reactions.Add(reaction.OutputType, reaction);
            }
        }
    }
}
