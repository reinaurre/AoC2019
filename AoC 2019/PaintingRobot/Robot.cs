using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace PaintingRobot
{
    public class Robot
    {
        public Coordinate RobotPosition;
        public Direction RobotFacing;

        public void Rotate(int command)
        {
            if (command == (int)Turn.Left)
            {
                switch (this.RobotFacing)
                {
                    case Direction.Up: this.RobotFacing = Direction.Left; break;
                    case Direction.Left: this.RobotFacing = Direction.Down; break;
                    case Direction.Down: this.RobotFacing = Direction.Right; break;
                    case Direction.Right: this.RobotFacing = Direction.Up; break;
                }
            }
            else if (command == (int)Turn.Right)
            {
                switch (this.RobotFacing)
                {
                    case Direction.Up: this.RobotFacing = Direction.Right; break;
                    case Direction.Left: this.RobotFacing = Direction.Up; break;
                    case Direction.Down: this.RobotFacing = Direction.Left; break;
                    case Direction.Right: this.RobotFacing = Direction.Down; break;
                }
            }
        }

        public void Walk(int distance)
        {
            switch (this.RobotFacing)
            {
                case Direction.Up:
                    for (int i = 1; i <= distance; i++)
                    {
                        this.RobotPosition.SetY(this.RobotPosition.Y - 1);
                    }
                    break;
                case Direction.Down:
                    for (int i = 1; i <= distance; i++)
                    {
                        this.RobotPosition.SetY(this.RobotPosition.Y + 1);
                    }
                    break;
                case Direction.Left:
                    for (int i = 1; i <= distance; i++)
                    {
                        this.RobotPosition.SetX(this.RobotPosition.X - 1);
                    }
                    break;
                case Direction.Right:
                    for (int i = 1; i <= distance; i++)
                    {
                        this.RobotPosition.SetX(this.RobotPosition.X + 1);
                    }
                    break;
            }
        }
    }
}
