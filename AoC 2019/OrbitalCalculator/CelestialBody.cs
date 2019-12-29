using System;
using System.Collections.Generic;
using Utilities;

namespace OrbitalCalculator
{
    public class CelestialBody
    {
        public string Name { get; private set; }
        public CelestialBody Parent { get; private set; }
        public List<CelestialBody> Satellites { get; private set; }
        public Coordinate3 Position { get; set; }
        public Coordinate3 Velocity { get; set; }
        public Coordinate3 LoopLength = new Coordinate3(0, 0, 0);
        public long LoopStep = 0;

        private Coordinate3 OriginalPosition;

        public CelestialBody(string name)
        {
            this.Name = name;
            this.Satellites = new List<CelestialBody>();
            this.Position = new Coordinate3(0, 0, 0);
            this.Velocity = new Coordinate3(0, 0, 0);
        }

        public CelestialBody(string name, int x, int y, int z)
        {
            this.Name = name;
            this.Position = new Coordinate3(x, y, z);
            this.Velocity = new Coordinate3(0, 0, 0);
            this.OriginalPosition = new Coordinate3(x, y, z);
        }

        public CelestialBody(string name, Coordinate3 position)
        {
            this.Name = name;
            this.Position = position;
            this.Velocity = new Coordinate3(0, 0, 0);
            this.OriginalPosition = new Coordinate3(position.X, position.Y, position.Z);
        }

        public CelestialBody(string name, CelestialBody parent, CelestialBody satellite)
        {
            this.Name = name;
            this.Parent = parent;
            this.Satellites = new List<CelestialBody>() { satellite };
            this.Position = new Coordinate3(0, 0, 0);
            this.Velocity = new Coordinate3(0, 0, 0);
        }

        public CelestialBody(string name, CelestialBody parent, List<CelestialBody> satellites)
        {
            this.Name = name;
            this.Parent = parent;
            this.Satellites = satellites;
            this.Position = new Coordinate3(0, 0, 0);
            this.Velocity = new Coordinate3(0, 0, 0);
        }

        public void Reset()
        {
            this.Position = this.OriginalPosition;
            this.Velocity = new Coordinate3(0, 0, 0);
        }

        public void AddSatellite(CelestialBody newSatellite)
        {
            this.Satellites.Add(newSatellite);
        }

        public void AddParent(CelestialBody parent)
        {
            this.Parent = parent;
        }

        public void ApplyVelocity(int stepNumber = -1)
        {
            this.Position.SetX(this.Position.X + this.Velocity.X);
            this.Position.SetY(this.Position.Y + this.Velocity.Y);
            this.Position.SetZ(this.Position.Z + this.Velocity.Z);
        }

        public void UpdateVelocity(Coordinate3 delta)
        {
            this.Velocity.SetX(this.Velocity.X + delta.X);
            this.Velocity.SetY(this.Velocity.Y + delta.Y);
            this.Velocity.SetZ(this.Velocity.Z + delta.Z);
        }

        public long GetEnergy()
        {
            int potential = Math.Abs(this.Position.X) + Math.Abs(this.Position.Y) + Math.Abs(this.Position.Z);
            int kinetic = Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) + Math.Abs(this.Velocity.Z);
            return potential * kinetic;
        }
    }
}
