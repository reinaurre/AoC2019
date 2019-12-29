using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utilities;

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

        public long GetRepeatStep()
        {
            List<CelestialBody> LoopsFoundList = new List<CelestialBody>();
            long stepNumber = 0;

            long minX = 0;
            long minY = 0;
            long minZ = 0;

            for (int axis = 0; axis < 3; axis++)
            {
                foreach(CelestialBody cb in this.celestialBodies)
                {
                    cb.Reset();
                }

                //while (LoopsFoundList.Count < this.celestialBodies.Count)
                while(stepNumber < double.MaxValue)
                {
                    this.ComparePositions();

                    foreach (CelestialBody cb in this.celestialBodies)
                    {
                        //cb.ApplyVelocity((int)stepNumber);
                        cb.ApplyVelocity();

                        //if (cb.LoopLength.X != 0 && cb.LoopLength.Y != 0 && cb.LoopLength.Z != 0 && !LoopsFoundList.Contains(cb))
                        //if (cb.LoopStep != 0 && !LoopsFoundList.Contains(cb))
                        //{
                        //    LoopsFoundList.Add(cb);
                        //}
                    }

                    bool match = true;

                    foreach (CelestialBody cb in this.celestialBodies)
                    {
                        match &= (axis == 0 && cb.Velocity.X == 0) || (axis == 1 && cb.Velocity.Y == 0) || (axis == 2 && cb.Velocity.Z == 0);
                    }

                    if (match)
                    {
                        if (axis == 0)
                        {
                            minX = stepNumber;
                        }
                        else if (axis == 1)
                        {
                            minY = stepNumber;
                        }
                        else if (axis == 2)
                        {
                            minZ = stepNumber;
                        }

                        break;
                    }

                    stepNumber++;
                }
            }

            //List<long> LCMs = new List<long>();

            //foreach(CelestialBody cb in this.celestialBodies)
            //{
            //    LCMs.Add(this.FindLCM(new long[] { cb.LoopLength.X, cb.LoopLength.Y, cb.LoopLength.Z }));
            //}

            //foreach (CelestialBody cb in this.celestialBodies)
            //{
            //    LCMs.Add(cb.LoopStep);
            //}

            //long temp = this.FindLCM(LCMs.ToArray());

            long temp = this.FindLCM(new long[] { minX, minY, minZ });
            return temp;
            // target: 4,686,774,924
        }

        private long FindLCM(long[] nums)
        {
            if(nums.Length < 2)
            {
                throw new ArgumentOutOfRangeException($"ERROR: Array length {nums.Length} too short to find LCM.");
            }

            long LCM = this.LeastCommonMultiple(nums[0], nums[1]);

            if(nums.Length == 2)
            {
                return LCM;
            }

            for(int i = 2; i < nums.Length; i++)
            {
                LCM = this.LeastCommonMultiple(LCM, nums[i]);
            }

            return LCM;
        }

        private long LeastCommonMultiple(long a, long b)
        {
            return a / this.GreatestCommonDivisor(a, b) * b;
        }

        private long GreatestCommonDivisor(long a, long b)
        {
            while (b != 0)
            {
                long t = b;
                b = a % b;
                a = t;
            }

            return a;
        }

        private int Step()
        {
            this.ComparePositions();

            int totalEnergy = 0;

            foreach (CelestialBody cb in this.celestialBodies)
            {
                cb.ApplyVelocity();

                totalEnergy += (int)cb.GetEnergy();
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
