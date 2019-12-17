using System;
using System.Collections.Generic;
using System.Text;

namespace WireManagement
{
    public class Command
    {
        public Direction Direction;
        public int Distance;

        public Command(Direction direction, int distance)
        {
            this.Direction = direction;
            this.Distance = distance;
        }
    }

    public enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R'
    }
}
