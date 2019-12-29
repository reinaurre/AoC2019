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

        public void Execute(int coins, bool interactive = false)
        {
            long[] inputs = new long[] { 0 };
            long[] output;
            long paddleX = 0;
            long ballX = 0;

            this.IC.ModifyMemoryValue(0, coins);

            while (IC.LatestOpcode != Opcodes.Terminate)
            {
                if (interactive)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.UpArrow: inputs[0] = 0; break;
                        case ConsoleKey.LeftArrow: inputs[0] = -1; break;
                        case ConsoleKey.RightArrow: inputs[0] = 1; break;
                        default: break;
                    }
                }
                else
                {
                    inputs[0] = (paddleX > ballX) ? -1 : (paddleX < ballX) ? 1 : 0;
                }

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
                long score = 0;

                if (xy.X == -1 && xy.Y == 0)
                {
                    score = output[2];
                }
                else
                {
                    Tile t = (Tile)Enum.Parse(typeof(Tile), output[2].ToString());

                    if(t == Tile.Ball)
                    {
                        ballX = xy.X;
                    }
                    else if(t == Tile.HPaddle)
                    {
                        paddleX = xy.X;
                    }

                    if (this.GameGrid.Count(kvp => kvp.Key.X == xy.X && kvp.Key.Y == xy.Y) == 0)
                    {
                        this.GameGrid.Add(new Coordinate(xy.X, xy.Y), t);
                    }
                    else
                    {
                        this.GameGrid[this.GameGrid.FirstOrDefault(kvp => kvp.Key.X == xy.X && kvp.Key.Y == xy.Y).Key] = t;
                    }
                }

                MapMaker MM = new MapMaker(this.GameGrid.Keys.ToList(), Node.Symbol.Empty);
                MM.PopulateGameGrid(this.GameGrid, score);
                MM.PrintLiveUpdates();
            }
        }
    }
}
