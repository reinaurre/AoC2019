using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime;
using System.Text;

namespace OrbitalCalculator
{
    public class SomeoneElsesAnswer_Day12Part2
    {
        public static Moon[] ProcessOrbit(Moon[] moons, double steps = double.MaxValue)
        {
            int mLen = moons.Length;

            double minx = 0;
            double miny = 0;
            double minz = 0;

            for (int a = 0; a < 3; a++)
            {
                foreach (Moon m in moons)
                {
                    m.Reset();
                }

                //Itterate the steps
                for (double i = 0; i < steps; i++)
                {

                    //Check each moon
                    for (int m1 = 0; m1 < mLen; m1++)
                    {
                        //Againt every other moon
                        for (int m2 = m1 + 1; m2 < mLen; m2++)
                        {
                            //Seems it'll run the loop first time regardless so need to break if we're at the end.
                            if (m2 >= mLen)
                            {
                                break;
                            }

                            if (a == 0)
                            {
                                ComperMoonsX(moons[m1], moons[m2]);
                            }
                            else if (a == 1)
                            {
                                ComperMoonsY(moons[m1], moons[m2]);
                            }
                            else if (a == 2)
                            {
                                ComperMoonsZ(moons[m1], moons[m2]);
                            }
                        }
                    }

                    //Console.WriteLine(String.Format("Step: {0}", i.ToString()));
                    //float enargy = 0;
                    foreach (Moon m in moons)
                    {
                        m.ApplyVelocity();
                        //enargy += m.GetEnargy();
                        //Console.WriteLine(m.ToString());
                    }

                    //Need do this just on each axis then get the Losest common mulitplier. Again stolen from other people!

                    bool match = true;
                    for (int m3 = 0; m3 < mLen; m3++)
                    {
                        //Can't take credit for this.
                        //Logically you don't have to check postion, we just have to know if the volcity is zero
                        match &= moons[m3].Velocity == Vector3.Zero;
                    }

                    if (match)
                    {
                        //Can't take credit for this either
                        //Planets move in symetrical cycles which means their volocity will reach zero at half the number of steps it'll take to get back to their orignal postion.

                        if (a == 0)
                        {
                            minx = i + 1;
                        }
                        else if (a == 1)
                        {
                            miny = i + 1;
                        }
                        else if (a == 2)
                        {
                            minz = i + 1;
                        }
                        break;
                    }
                }
            }

            Console.WriteLine(minx.ToString());
            Console.WriteLine(miny.ToString());
            Console.WriteLine(minz.ToString());

            Console.WriteLine((LCM(minx, LCM(miny, minz)) * 2).ToString());

            return moons;
        }

        static double GCD(double a, double b)
        {
            if (a % b == 0) return b;
            return GCD(b, a % b);
        }

        static double LCM(double a, double b)
        {
            return a * b / GCD(a, b);
        }

        static void ComperMoons(Moon moonA, Moon moonB)
        {
            ComperMoonsX(moonA, moonB);
            ComperMoonsY(moonA, moonB);
            ComperMoonsZ(moonA, moonB);
        }

        //Axises are indepent so we should split this out really 
        //Just doing one for testing now
        static void ComperMoonsX(Moon moonA, Moon moonB)
        {
            //need three way logic here for ==s
            int vx = 0;

            vx = moonA.Postion.X < moonB.Postion.X ? 1 : -1;
            vx = moonA.Postion.X == moonB.Postion.X ? 0 : vx;


            Vector3 va = new Vector3(vx, 0, 0);

            moonA.AddVelocity(va);

            Vector3 vb = new Vector3(vx * -1, 0, 0);

            moonB.AddVelocity(vb);
        }

        static void ComperMoonsY(Moon moonA, Moon moonB)
        {
            //need three way logic here for ==s

            int vy = 0;

            vy = moonA.Postion.Y < moonB.Postion.Y ? 1 : -1;
            vy = moonA.Postion.Y == moonB.Postion.Y ? 0 : vy;


            Vector3 va = new Vector3(0, vy, 0);

            moonA.AddVelocity(va);

            Vector3 vb = new Vector3(0, vy * -1, 0);

            moonB.AddVelocity(vb);
        }

        static void ComperMoonsZ(Moon moonA, Moon moonB)
        {
            //need three way logic here for ==s
            int vz = 0;
            vz = moonA.Postion.Z < moonB.Postion.Z ? 1 : -1;
            vz = moonA.Postion.Z == moonB.Postion.Z ? 0 : vz;

            Vector3 va = new Vector3(0, 0, vz);

            moonA.AddVelocity(va);

            Vector3 vb = new Vector3(0, 0, vz * -1);

            moonB.AddVelocity(vb);
        }

    }

    public class Moon
    {
        public char ID { get; protected set; }
        public Vector3 Postion { get; protected set; }
        public Vector3 Velocity { get; protected set; }

        public Vector3 StartingPos { get; protected set; }

        public Moon(char id, Vector3 startingPos)
        {
            ID = id;
            Postion = startingPos;
            StartingPos = startingPos;
            Velocity = Vector3.Zero;
        }

        public void AddVelocity(Vector3 v)
        {
            Velocity = Vector3.Add(Velocity, v);
        }

        public void ApplyVelocity()
        {
            Postion = Vector3.Add(Postion, Velocity);
        }

        public float GetEnargy()
        {
            float p = Math.Abs(Postion.X) + Math.Abs(Postion.Y) + Math.Abs(Postion.Z);
            float v = Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

            return p * v;
        }

        public override string ToString()
        {
            return String.Format("[{6}] pos=<x=-{0}, y=  {1}, z= {2}>, vel=<x= {3}, y= {4}, z= {5}>"
                , Postion.X, Postion.Y, Postion.Z, Velocity.X, Velocity.Y, Velocity.Z, ID.ToString());
        }

        public void Reset()
        {
            Postion = StartingPos;
            Velocity = Vector3.Zero;
        }
    }

}