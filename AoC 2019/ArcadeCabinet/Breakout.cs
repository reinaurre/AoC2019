using Intcode_Computer;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace ArcadeCabinet
{
    public class Breakout
    {
        public Dictionary<Coordinate, Tile> GameGrid = new Dictionary<Coordinate, Tile>();
        private IntcodeComputer IC;

        public Breakout(string intcodeInput)
        {
            this.IC = new IntcodeComputer(intcodeInput);
        }

        public void Execute()
        {
            long[] inputs = new long[] { 0 };
            long[] output;

            while (IC.LatestOpcode != Opcodes.Terminate)
            {
                output = this.IC.ExecuteUntilOutputNumber(inputs, 3);

                if (IC.LatestOpcode == Opcodes.Terminate)
                {
                    break;
                }

                if(output.Length != 3)
                {
                    throw new ArgumentOutOfRangeException("Intcode Computer Output", $"ERROR: Intcode Computer gave only {output.Length} values when 3 are expected.");
                }

                Coordinate xy = new Coordinate((int)output[0], (int)output[1]);
                Tile t = (Tile)Enum.Parse(typeof(Tile), output[2].ToString());

                if (this.GameGrid.Count(kvp => kvp.Key.X == xy.X && kvp.Key.Y == xy.Y) == 0)
                {
                    this.GameGrid.Add(new Coordinate(xy.X, xy.Y), t);
                }
                else
                {
                    this.GameGrid[this.GameGrid.FirstOrDefault(kvp => kvp.Key.X == xy.X && kvp.Key.Y == xy.Y).Key] = t;
                }

                MapMaker MM = new MapMaker(this.GameGrid.Keys.ToList(), Node.Symbol.Empty);
                MM.PopulateGameGrid(this.GameGrid);
                MM.PrintWholeMap();
            }
        }
    }
}
