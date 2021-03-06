﻿using System;

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
        Right = 'R',
        Forward = 'F',
        Back = 'B'
    }

    public enum Symbol
    {
        Empty = ' ',
        O = 'O',
        BigX = 'X',
        SmallX = '×',
        Box = '■',
        Dot = '.',
        Hash = '#',
        VLines = '║',
        HLines = '═'
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
