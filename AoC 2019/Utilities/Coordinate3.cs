namespace Utilities
{
    public class Coordinate3 : Coordinate
    {
        public int Z { get; private set; }

        public Coordinate3(int x, int y, int z) : base(x, y)
        {
            this.Z = z;
        }

        public void SetZ(int z)
        {
            this.Z = z;
        }
    }
}
