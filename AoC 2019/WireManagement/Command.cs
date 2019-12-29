using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

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
}
