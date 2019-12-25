using System.Collections.Generic;
using WireManagement;

namespace MonitoringStation
{
    public class Asteroid
    {
        public double AngleToStation { get; set; }
        public Coordinate Coordinate { get; private set; }
        public List<Asteroid> AsteroidsDetected { get; set; }

        public Asteroid(int x, int y)
        {
            this.AsteroidsDetected = new List<Asteroid>();
            this.Coordinate = new Coordinate(x, y);
        }
    }
}
