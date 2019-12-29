using System;

namespace Utilities
{
    public enum Color
    {
        None = -1,
        Black = 0,
        White = 1
    }

    public enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R'
    }

    public enum Tile
    {
        Empty = 0,
        Wall = 1,
        Block = 2,
        HPaddle = 3,
        Ball = 4
    }

    public enum Turn
    {
        Left = 0,
        Right = 1
    }
}
