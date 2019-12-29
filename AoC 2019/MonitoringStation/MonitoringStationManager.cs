using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace MonitoringStation
{
    public class MonitoringStationManager
    {
        public char[,] Grid;
        public Asteroid MonitoringStation;

        private List<Asteroid> AsteroidList = new List<Asteroid>();
        private double LaserAngle;
        private int lastDestroyedIndex = -1;

        public void BuildGrid(string[] input)
        {
            this.Grid = new char[input[0].Length, input.Length];

            for (int y = 0; y < input.Length; y++)
            {
                char[] row = input[y].ToCharArray();
                for (int x = 0; x < row.Length; x++)
                {
                    this.Grid[x, y] = row[x];

                    if (this.Grid[x, y] == '#')
                    {
                        this.AsteroidList.Add(new Asteroid(x, y));
                    }
                }
            }
        }

        public int SetMonitoringStationLocation()
        {
            this.MonitoringStation = this.FindMostAsteroidsDetected();
            return this.MonitoringStation.AsteroidsDetected.Count;
        }

        public void InitializeLaser(double startingAngle = -1.5707963267948966)
        {
            this.LaserAngle = startingAngle;
            this.CalculateAsteroidAnglesToStation();
            this.ArrangeDetectedAsteroidsByAngle();
        }

        public Coordinate FireLaser()
        {
            Coordinate asteroidDestroyed = this.MonitoringStation.Coordinate;

            if(this.MonitoringStation.AsteroidsDetected.Count == 0)
            {
                this.FindLOS(this.MonitoringStation);
            }

            int index = this.GetNextTarget();

            if (this.MonitoringStation.AsteroidsDetected.Count > 0)
            {
                asteroidDestroyed = this.MonitoringStation.AsteroidsDetected[index].Coordinate;
                this.LaserAngle = this.MonitoringStation.AsteroidsDetected[index].AngleToStation;

                this.Grid[asteroidDestroyed.X, asteroidDestroyed.Y] = '.';
                this.MonitoringStation.AsteroidsDetected.Remove(this.MonitoringStation.AsteroidsDetected[index]);
            }

            return asteroidDestroyed;
        }

        private int GetNextTarget()
        {
            if(this.lastDestroyedIndex >= this.MonitoringStation.AsteroidsDetected.Count)
            {
                return 0;
            }
            else if (this.lastDestroyedIndex != -1)
            {
                return this.lastDestroyedIndex;
            }

            double minAngle = 99;
            int minIndex = 0;

            for(int i = 0; i < this.MonitoringStation.AsteroidsDetected.Count; i++)
            {
                double angle = this.MonitoringStation.AsteroidsDetected[i].AngleToStation;

                if (angle >= this.LaserAngle && angle < minAngle)
                {
                    minAngle = angle;
                    minIndex = i;
                    this.lastDestroyedIndex = minIndex;
                    return minIndex;
                }
            }

            return minIndex;
        }

        private void CalculateAsteroidAnglesToStation()
        {
            foreach(Asteroid asteroid in this.MonitoringStation.AsteroidsDetected)
            {
                Coordinate relative = new Coordinate(asteroid.Coordinate.X - this.MonitoringStation.Coordinate.X, 
                    asteroid.Coordinate.Y - this.MonitoringStation.Coordinate.Y);

                asteroid.AngleToStation = Math.Atan2(relative.Y, relative.X);
            }
        }

        private void ArrangeDetectedAsteroidsByAngle()
        {
            this.MonitoringStation.AsteroidsDetected = this.MonitoringStation.AsteroidsDetected.OrderBy(x => x.AngleToStation).ToList();
        }

        private Asteroid FindMostAsteroidsDetected()
        {
            Asteroid max = new Asteroid(0, 0);
            foreach (Asteroid asteroid in this.AsteroidList)
            {
                this.FindLOS(asteroid);

                if (asteroid.AsteroidsDetected.Count > max.AsteroidsDetected.Count)
                {
                    max = asteroid;
                }
            }

            return max;
        }

        private void FindLOS(Asteroid root)
        {
            int Xmax = Grid.GetLength(0);
            int Ymax = Grid.GetLength(1);

            // Check the cardinal axes first
            if (root.Coordinate.X + 1 < Xmax)
            {
                for (int i = root.Coordinate.X + 1; i < Xmax; i++)
                {
                    if (this.Grid[i, root.Coordinate.Y] == '#')
                    {
                        root.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == i && a.Coordinate.Y == root.Coordinate.Y));
                        break;
                    }
                }
            }

            if (root.Coordinate.X - 1 >= 0)
            {
                for (int i = root.Coordinate.X - 1; i >= 0; i--)
                {
                    if (this.Grid[i, root.Coordinate.Y] == '#')
                    {
                        root.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == i && a.Coordinate.Y == root.Coordinate.Y));
                        break;
                    }
                }
            }

            if (root.Coordinate.Y - 1 >= 0)
            {
                for (int i = root.Coordinate.Y - 1; i >= 0; i--)
                {
                    if (this.Grid[root.Coordinate.X, i] == '#')
                    {
                        root.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == root.Coordinate.X && a.Coordinate.Y == i));
                        break;
                    }
                }
            }

            if (root.Coordinate.Y + 1 < Ymax)
            {
                for (int i = root.Coordinate.Y + 1; i < Ymax; i++)
                {
                    if (this.Grid[root.Coordinate.X, i] == '#')
                    {
                        root.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == root.Coordinate.X && a.Coordinate.Y == i));
                        break;
                    }
                }
            }

            // Now start going through the diagonals and their cardinal axes
            // NE
            if (root.Coordinate.X + 1 < Xmax && root.Coordinate.Y - 1 >= 0)
            {
                this.FindLOSRecursively(root, new Coordinate(root.Coordinate.X + 1, root.Coordinate.Y - 1), 1, -1);
            }
            // SE
            if (root.Coordinate.X + 1 < Xmax && root.Coordinate.Y + 1 < Ymax)
            {
                this.FindLOSRecursively(root, new Coordinate(root.Coordinate.X + 1, root.Coordinate.Y + 1), 1, 1);
            }
            // SW
            if (root.Coordinate.X - 1 >= 0 && root.Coordinate.Y + 1 < Ymax)
            {
                this.FindLOSRecursively(root, new Coordinate(root.Coordinate.X - 1, root.Coordinate.Y + 1), -1, 1);
            }
            // NW
            if (root.Coordinate.X - 1 >= 0 && root.Coordinate.Y - 1 >= 0)
            {
                this.FindLOSRecursively(root, new Coordinate(root.Coordinate.X - 1, root.Coordinate.Y - 1), -1, -1);
            }
        }

        private void FindLOSRecursively(Asteroid origin, Coordinate root, int quadSignX, int quadSignY)
        {
            int Xmax = this.Grid.GetLength(0);
            int Ymax = this.Grid.GetLength(1);

            if (this.Grid[root.X, root.Y] == '#' && !this.IsLOSBlocked(origin, root.X, root.Y))
            {
                origin.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == root.X && a.Coordinate.Y == root.Y));
            }

            if (quadSignX > 0 && root.X + 1 < Xmax)
            {
                for (int i = root.X + 1; i < Xmax; i++)
                {
                    if (this.Grid[i, root.Y] == '#' && !this.IsLOSBlocked(origin, i, root.Y))
                    {
                        origin.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == i && a.Coordinate.Y == root.Y));
                    }
                }
            }

            if (quadSignX < 0 && root.X - 1 >= 0)
            {
                for (int i = root.X - 1; i >= 0; i--)
                {
                    if (this.Grid[i, root.Y] == '#' && !this.IsLOSBlocked(origin, i, root.Y))
                    {
                        origin.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == i && a.Coordinate.Y == root.Y));
                    }
                }
            }

            if (quadSignY < 0 && root.Y - 1 >= 0)
            {
                for (int i = root.Y - 1; i >= 0; i--)
                {
                    if (this.Grid[root.X, i] == '#' && !this.IsLOSBlocked(origin, root.X, i))
                    {
                        origin.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == root.X && a.Coordinate.Y == i));
                    }
                }
            }

            if (quadSignY > 0 && root.Y + 1 < Ymax)
            {
                for (int i = root.Y + 1; i < Ymax; i++)
                {
                    if (this.Grid[root.X, i] == '#' && !this.IsLOSBlocked(origin, root.X, i))
                    {
                        origin.AsteroidsDetected.Add(this.AsteroidList.First(a => a.Coordinate.X == root.X && a.Coordinate.Y == i));
                    }
                }
            }

            // NE
            if (quadSignX > 0 && quadSignY < 0 && root.X + 1 < Xmax && root.Y - 1 >= 0)
            {
                this.FindLOSRecursively(origin, new Coordinate(root.X + 1, root.Y - 1), quadSignX, quadSignY);
            }
            // SE
            else if (quadSignX > 0 && quadSignY > 0 && root.X + 1 < Xmax && root.Y + 1 < Ymax)
            {
                this.FindLOSRecursively(origin, new Coordinate(root.X + 1, root.Y + 1), quadSignX, quadSignY);
            }
            // SW
            else if (quadSignX < 0 && quadSignY > 0 && root.X - 1 >= 0 && root.Y + 1 < Ymax)
            {
                this.FindLOSRecursively(origin, new Coordinate(root.X - 1, root.Y + 1), quadSignX, quadSignY);
            }
            // NW
            else if (quadSignX < 0 && quadSignY < 0 && root.X - 1 >= 0 && root.Y - 1 >= 0)
            {
                this.FindLOSRecursively(origin, new Coordinate(root.X - 1, root.Y - 1), quadSignX, quadSignY);
            }
        }

        private bool IsLOSBlocked(Asteroid origin, int x, int y)
        {
            return origin.AsteroidsDetected.Where(ast => ast.Coordinate.X != 0 && ast.Coordinate.Y != 0)
                .Count(a =>
                {
                    Coordinate oldA = new Coordinate(a.Coordinate.X - origin.Coordinate.X, a.Coordinate.Y - origin.Coordinate.Y);
                    Coordinate newA = new Coordinate(x - origin.Coordinate.X, y - origin.Coordinate.Y);

                    double oldAngle = Math.Atan2(oldA.Y, oldA.X);
                    double newAngle = Math.Atan2(newA.Y, newA.X);

                    return oldAngle == newAngle;
                }) > 0;
        }
    }
}
