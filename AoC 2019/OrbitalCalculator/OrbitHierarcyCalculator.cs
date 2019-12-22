using System;
using System.Collections.Generic;
using System.Linq;

namespace OrbitalCalculator
{
    public class OrbitHierarchyCalculator
    {
        private HashSet<CelestialBody> celestialBodies = new HashSet<CelestialBody>();

        public int SumAllOrbitRelationships(string[] inputArr)
        {
            foreach(string value in inputArr)
            {
                this.ParseRelationshipFromString(value);
            }

            int sum = 0;

            foreach(CelestialBody node in this.celestialBodies)
            {
                sum += this.SumRelationshipChain(node);
            }

            return sum;
        }

        public int FindShortestValidPath(string [] inputArr)
        {
            foreach(string value in inputArr)
            {
                this.ParseRelationshipFromString(value);
            }

            CelestialBody you = this.celestialBodies.First(x => x.Name == "YOU");
            CelestialBody santa = this.celestialBodies.First(x => x.Name == "SAN");

            return this.FindCommonAncestor(you, santa);
        }

        private int SumRelationshipChain(CelestialBody current)
        {
            int count = 0;

            while (current.Name != "COM")
            {
                current = current.Parent;
                count++;
            }

            return count;
        }

        private int FindCommonAncestor(CelestialBody you, CelestialBody san)
        {
            Dictionary<CelestialBody, int> youAncestors = new Dictionary<CelestialBody, int>();
            CelestialBody current = you.Parent;
            int depthCount = 0;

            while(current.Name != "COM")
            {
                youAncestors.Add(current, depthCount);
                current = current.Parent;
                depthCount++;
            }

            depthCount = 0;
            current = san.Parent;

            while(current.Name != "COM")
            {
                if (youAncestors.ContainsKey(current))
                {
                    return depthCount + youAncestors.Where(x => x.Key == current).First().Value;
                }

                current = current.Parent;
                depthCount++;
            }

            return -1;
        }

        private void ParseRelationshipFromString(string input)
        {
            string[] names = input.Split(')');

            if(this.celestialBodies.Count(x => x.Name == names[0]) == 0)
            {
                this.celestialBodies.Add(new CelestialBody(names[0]));
            }

            if(this.celestialBodies.Count(x => x.Name == names[1]) == 0)
            {
                this.celestialBodies.Add(new CelestialBody(names[1]));
            }

            this.celestialBodies.First(x => x.Name == names[0]).AddSatellite(this.celestialBodies.First(x => x.Name == names[1]));
            this.celestialBodies.First(x => x.Name == names[1]).AddParent(this.celestialBodies.First(x => x.Name == names[0]));
        }
    }
}
