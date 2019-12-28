using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WireManagement;

namespace OrbitalCalculator
{
    public class OrbitalEnergyCalculator
    {
        public List<CelestialBody> celestialBodies = new List<CelestialBody>();

        private long tickLimit = 0;
        private static readonly string[] planetNames = { "Io", "Europa", "Ganymede", "Callisto" };

        public OrbitalEnergyCalculator(long tickLimit, string[] rawPositions)
        {
            this.tickLimit = tickLimit;
            
            for(int i = 0; i < rawPositions.Length; i++)
            {
                this.celestialBodies.Add(new CelestialBody(planetNames[i], rawPositions[i].ToCoordinate3()));
            }
        }

        public int GetTotalEnergy()
        {
            int output = 0;

            for(long i = 0; i < this.tickLimit; i++)
            {
                if (i == 0 || i % 100 == 0)
                {
                    Console.WriteLine($"After {i} steps:");
                    foreach (CelestialBody cb in this.celestialBodies)
                    {
                        Console.Write($"pos=<x={cb.Position.X}, y={cb.Position.Y}, z={cb.Position.Z}>, ");
                        Console.Write($"vel=<x={cb.Velocity.X}, y={cb.Velocity.Y}, z={cb.Velocity.Z}>");
                        Console.WriteLine();
                    }
                }

                output = this.Step();
            }

            Console.WriteLine($"After {this.tickLimit} steps:");
            foreach (CelestialBody cb in this.celestialBodies)
            {
                Console.Write($"pos=<x={cb.Position.X}, y={cb.Position.Y}, z={cb.Position.Z}>, ");
                Console.Write($"vel=<x={cb.Velocity.X}, y={cb.Velocity.Y}, z={cb.Velocity.Z}>");
                Console.WriteLine();
            }

            return output;
        }

        private int Step()
        {
            this.ComparePositions();

            int totalEnergy = 0;
            bool isRepeat = true;

            foreach (CelestialBody cb in this.celestialBodies)
            {
                bool temp = cb.ApplyVelocity();

                if (isRepeat)
                {
                    isRepeat = temp;
                }

                int potential = Math.Abs(cb.Position.X) + Math.Abs(cb.Position.Y) + Math.Abs(cb.Position.Z);
                int kinetic = Math.Abs(cb.Velocity.X) + Math.Abs(cb.Velocity.Y) + Math.Abs(cb.Velocity.Z);

                totalEnergy += potential * kinetic;
            }

            if (isRepeat)
            {

            }

            return totalEnergy;
        }

        private void ComparePositions()
        {
            for(int cb = 0; cb < this.celestialBodies.Count; cb++)
            {
                Coordinate3 delta = new Coordinate3(0, 0, 0);

                for (int i = 0; i < this.celestialBodies.Count; i++)
                {
                    if(i == cb)
                    {
                        continue;
                    }

                    if(this.celestialBodies[i].Position.X > this.celestialBodies[cb].Position.X)
                    {
                        delta.SetX(delta.X + 1);
                    }
                    else if(this.celestialBodies[i].Position.X < this.celestialBodies[cb].Position.X)
                    {
                        delta.SetX(delta.X - 1);
                    }

                    if (this.celestialBodies[i].Position.Y > this.celestialBodies[cb].Position.Y)
                    {
                        delta.SetY(delta.Y + 1);
                    }
                    else if (this.celestialBodies[i].Position.Y < this.celestialBodies[cb].Position.Y)
                    {
                        delta.SetY(delta.Y - 1);
                    }

                    if (this.celestialBodies[i].Position.Z > this.celestialBodies[cb].Position.Z)
                    {
                        delta.SetZ(delta.Z + 1);
                    }
                    else if (this.celestialBodies[i].Position.Z < this.celestialBodies[cb].Position.Z)
                    {
                        delta.SetZ(delta.Z - 1);
                    }
                }

                this.celestialBodies[cb].UpdateVelocity(delta);
            }
        }
    }

    public static class OrbitEx
    {
        public static Coordinate3 ToCoordinate3(this string input)
        {
            // input looks like: <x=-1, y=0, z=2>
            string regex = "[x|y|z]=(-*[0-9]+)";
            MatchCollection mc = Regex.Matches(input, regex);

            if(mc.Count != 3)
            {
                throw new ArgumentException($"ERROR: Invalid string {input}. Can't convert to Coordinate3");
            }

            return new Coordinate3(Convert.ToInt32(mc[0].Groups[1].Value), Convert.ToInt32(mc[1].Groups[1].Value), Convert.ToInt32(mc[2].Groups[1].Value));
        }
    }
}
