using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChemicalReactor
{
    class Reaction
    {
        public string OutputType { get; private set; }
        public int OutputQuantity { get; private set; }

        public Dictionary<string, int> Inputs { get; private set; }

        public Reaction(string reactionString)
        {
            this.Inputs = new Dictionary<string, int>();
            this.ParseFormula(reactionString);
        }

        private void ParseFormula(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"(\d+\s\w+)+");
            if(matches.Count <= 1)
            {
                throw new Exception("too few results");
            }

            for(int i = 0; i < matches.Count; i++)
            {
                string[] temp = matches[i].Value.Split(' ');

                if(temp.Length != 2)
                {
                    throw new Exception("wrong substring pair");
                }
                int num = Convert.ToInt32(temp[0]);

                if(i == matches.Count - 1)
                {
                    this.OutputType = temp[1];
                    this.OutputQuantity = num;
                }
                else
                {
                    this.Inputs.Add(temp[1], num);
                }
            }
        }
    }
}
