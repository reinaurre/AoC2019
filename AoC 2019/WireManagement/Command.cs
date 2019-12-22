using System;
using System.Collections.Generic;
using System.Text;

namespace WireManagement
{
    public class Command
    {
        public Direction Direction { get; private set; }
        public int Distance { get; private set; }

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
