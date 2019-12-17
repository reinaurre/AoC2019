using System;
using System.Collections.Generic;
using System.Text;

namespace WireManagement
{
    public class Intersection : Coordinate
    {
        public int LengthA;
        public int LengthB;

        /// <summary>
        /// Initializes a new Intersection object
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Intersection(int x, int y) : base(x, y) { }

        /// <summary>
        /// Initializes a new Intersection object
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="lengthA">Length of first path leading to this intersection.</param>
        /// <param name="lengthB">Length of second path leading to this intersection.</param>
        public Intersection(int x, int y, int lengthA, int lengthB) : base(x, y)
        {
            this.LengthA = lengthA;
            this.LengthB = lengthB;
        }
    }
}
