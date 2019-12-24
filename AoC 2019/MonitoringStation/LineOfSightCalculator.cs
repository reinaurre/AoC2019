using System;
using System.Collections.Generic;
using System.Linq;
using WireManagement;

namespace MonitoringStation
{
    public class LineOfSightCalculator
    {
        public char[,] Grid;
        public Asteroid MonitoringStation;

        private List<Asteroid> AsteroidList = new List<Asteroid>();

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

        public int FindMostAsteroidsDetected()
        {
            int max = 0;
            foreach (Asteroid asteroid in this.AsteroidList)
            {
                this.FindLOS(asteroid);

                //if (asteroid.Coordinate.X == 3 && asteroid.Coordinate.Y == 4)
                //{
                //    this.MonitoringStation = asteroid;
                //    max = asteroid.AsteroidsDetected.Count;
                //}

                if (asteroid.AsteroidsDetected.Count > max)
                {
                    this.MonitoringStation = asteroid;
                    max = asteroid.AsteroidsDetected.Count;
                }
            }

            var duplicateKeys = this.MonitoringStation.AsteroidsDetected.GroupBy(x => new { x.Coordinate.X, x.Coordinate.Y })
                        .Where(group => group.Count() > 1);

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
                    if ((a.Coordinate.X > origin.Coordinate.X && x < origin.Coordinate.X)
                    || (a.Coordinate.X < origin.Coordinate.X && x > origin.Coordinate.X)
                    || (a.Coordinate.Y > origin.Coordinate.Y && y < origin.Coordinate.Y)
                    || (a.Coordinate.Y < origin.Coordinate.Y && y > origin.Coordinate.Y))
                    {
                        return false;
                    }


                    Coordinate oldA = new Coordinate(a.Coordinate.X - origin.Coordinate.X, a.Coordinate.Y - origin.Coordinate.Y);
                    Coordinate newA = new Coordinate(x - origin.Coordinate.X, y - origin.Coordinate.Y);

                    //if ((newA.X == 0 && oldA.X != 0)
                    //|| (oldA.X == 0 && newA.X != 0)
                    //|| (newA.Y == 0 && oldA.Y != 0)
                    //|| (oldA.Y == 0 && newA.Y != 0)
                    //|| Math.Abs(oldA.X) >= Math.Abs(newA.X)
                    //|| Math.Abs(oldA.Y) >= Math.Abs(newA.Y))
                    if ((newA.X == 0 && oldA.X != 0)
                    || (oldA.X == 0 && newA.X != 0)
                    || (newA.Y == 0 && oldA.Y != 0)
                    || (oldA.Y == 0 && newA.Y != 0))
                    //if (oldA.X == 0 || oldA.Y == 0)
                    {
                        return false;
                    }

                    //var val1 = (float)newA.X / oldA.X;
                    //var val2 = (float)newA.Y / oldA.Y;
                    //int val3 = newA.X % oldA.X;
                    //int val4 = newA.Y % oldA.Y;

                    return (newA.X % oldA.X == 0 && newA.Y % oldA.Y == 0
                        && (float)newA.X / oldA.X == (float)newA.Y / oldA.Y)
                        || (Math.Abs(newA.X) == Math.Abs(newA.Y) && Math.Abs(oldA.X) == Math.Abs(oldA.Y));
                }) > 0;
        }
    }
}
