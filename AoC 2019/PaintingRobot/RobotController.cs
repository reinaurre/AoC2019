using Intcode_Computer;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace PaintingRobot
{
    public class RobotController : Robot
    {
        public Dictionary<Coordinate, Color> PaintMap = new Dictionary<Coordinate, Color>();
        public int NumberPainted = 0;
        private IntcodeComputer IC;

        public RobotController(int x, int y, string program)
        {
            base.RobotPosition = new Coordinate(x, y);
            base.RobotFacing = Direction.Up;
            this.IC = new IntcodeComputer(program);
            this.PaintMap.Add(new Coordinate(base.RobotPosition.X, base.RobotPosition.Y), Color.Black);
        }

        public void ExecutePaintingProgram()
        {
            long[] inputs = new long[] { 1 };
            long[] output;

            while (IC.LatestOpcode != Opcodes.Terminate)
            {
                output = this.IC.ExecuteUntilOutputNumber(inputs, 2);

                if(IC.LatestOpcode == Opcodes.Terminate)
                {
                    break;
                }

                // order: Paint, Turn, Move
                this.Paint((Color)Enum.Parse(typeof(Color),output[0].ToString()));
                base.Rotate((int)output[1]);
                base.Walk(1);
                if (this.PaintMap.Count(kvp => kvp.Key.X == this.RobotPosition.X && kvp.Key.Y == this.RobotPosition.Y) == 0)
                {
                    this.PaintMap.Add(new Coordinate(this.RobotPosition.X, this.RobotPosition.Y), Color.Black);
                }

                // if current node == black input = 0
                // white input = 1
                inputs[0] = (int)this.PaintMap[this.PaintMap.FirstOrDefault(kvp => kvp.Key.X == base.RobotPosition.X && kvp.Key.Y == base.RobotPosition.Y).Key];
            }
        }

        private void Paint(Color newColor)
        {
            this.PaintMap[this.PaintMap.FirstOrDefault(kvp => kvp.Key.X == base.RobotPosition.X && kvp.Key.Y == base.RobotPosition.Y).Key] = newColor;
            this.NumberPainted++;
        }
    }
}
