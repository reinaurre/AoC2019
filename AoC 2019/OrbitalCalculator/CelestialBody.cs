using System;
using System.Collections.Generic;

namespace OrbitalCalculator
{
    class CelestialBody
    {
        public string Name { get; private set; }
        public CelestialBody Parent { get; private set; }
        public List<CelestialBody> Satellites { get; private set; }

        public CelestialBody(string name)
        {
            this.Name = name;
            this.Satellites = new List<CelestialBody>();
        }

        public CelestialBody(string name, CelestialBody parent, CelestialBody satellite)
        {
            this.Name = name;
            this.Parent = parent;
            this.Satellites = new List<CelestialBody>() { satellite };
        }

        public CelestialBody(string name, CelestialBody parent, List<CelestialBody> satellites)
        {
            this.Name = name;
            this.Parent = parent;
            this.Satellites = satellites;
        }

        public void AddSatellite(CelestialBody newSatellite)
        {
            this.Satellites.Add(newSatellite);
        }

        public void AddParent(CelestialBody parent)
        {
            this.Parent = parent;
        }
    }
}
