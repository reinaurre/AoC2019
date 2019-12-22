using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WireManagement
{
    public class Intersection : Coordinate
    {
        public bool isSelfIntersection { get; private set; }
        public int LengthA;
        public int LengthB;
        public int ManhattanDistance;

        /// <summary>
        /// Initializes a new Intersection object
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="isSelf">True if this is a self-intersection.</param>
        public Intersection(int x, int y, bool isSelf) : base(x, y) {
            this.isSelfIntersection = isSelf;
        }

        /// <summary>
        /// Initializes a new Intersection object
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="isSelf">True if this is a self-intersection.</param>
        /// <param name="lengthA">Length of first path leading to this intersection.</param>
        /// <param name="lengthB">Length of second path leading to this intersection.</param>
        public Intersection(int x, int y, bool isSelf, int lengthA, int lengthB) : base(x, y)
        {
            this.isSelfIntersection = isSelf;
            this.LengthA = lengthA;
            this.LengthB = lengthB;
        }
    }

    public static class IntersectionEx
    {
        public static List<Intersection> SortByManhattanDistance(this List<Intersection> input)
        {
            return input.OrderBy(i => i.ManhattanDistance).ToList();
        }
    }
}
