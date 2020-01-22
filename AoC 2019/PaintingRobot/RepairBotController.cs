using Intcode_Computer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace RemoteBotControl
{
    public class RepairBotController
    {
        public Dictionary<Coordinate, Symbol> Map = new Dictionary<Coordinate, Symbol>();
        private Coordinate BotPosition;
        private Coordinate TargetPosition;
        private Coordinate previousLocation;
        private Direction Heading;
        private IntcodeComputer IC;

        private int minX = 0;
        private int maxX = 0;
        private int minY = 0;
        private int maxY = 0;
        private long totalArea = long.MaxValue;

        public RepairBotController(int x, int y, string program)
        {
            this.BotPosition = new Coordinate(x, y);
            this.Heading = Direction.Up;
            this.IC = new IntcodeComputer(program);
            this.Map.Add(new Coordinate(this.BotPosition.X, this.BotPosition.Y), Symbol.Dot);
        }

        public void ExecuteWanderProgram()
        {
            long[] inputs = new long[] { 1 };
            long[] outputVals;
            this.previousLocation = this.Map.Keys.First();
            int counter = 0;

            while (this.Map.Count < this.totalArea - (Math.Ceiling(this.totalArea * 0.01) + 5)) // +5 is for the corners and rounding up to the nearest whole
            {
                outputVals = this.IC.ExecuteUntilOutputNumber(inputs, 1);

                if(outputVals.Length == 0)
                {
                    return;
                }

                int output = (int)outputVals[0];

                Symbol outputSymbol = this.CheckForExisting(inputs[0]);

                switch (output)
                {
                    case 0: // wall
                        if (outputSymbol == Symbol.Empty)
                        {
                            this.AddWall(inputs[0]);
                        }
                        break;
                    case 1: // success
                        this.previousLocation = new Coordinate(this.BotPosition.X, this.BotPosition.Y);
                        this.UpdateBotPosition(inputs[0]);
                        Coordinate newLocation = new Coordinate(this.BotPosition.X, this.BotPosition.Y);
                        if (outputSymbol == Symbol.Empty)
                        {
                            this.Map.Add(newLocation, Symbol.SmallX);
                        }
                        else
                        {
                            this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == newLocation.X && kvp.Key.Y == newLocation.Y).Key] = Symbol.SmallX;
                        }
                        if(this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == this.previousLocation.X && kvp.Key.Y == this.previousLocation.Y).Key] != Symbol.O)
                        {
                            this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == this.previousLocation.X && kvp.Key.Y == this.previousLocation.Y).Key] = Symbol.Dot;
                        }
                        break;
                    case 2: // target
                        this.previousLocation = new Coordinate(this.BotPosition.X, this.BotPosition.Y);
                        this.UpdateBotPosition(inputs[0]);
                        this.TargetPosition = new Coordinate(this.BotPosition.X, this.BotPosition.Y);
                        if (outputSymbol == Symbol.Empty)
                        {
                            this.Map.Add(this.TargetPosition, Symbol.O);
                        }
                        else
                        {
                            this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == this.TargetPosition.X && kvp.Key.Y == this.TargetPosition.Y).Key] = Symbol.O;
                        }
                        this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == this.previousLocation.X && kvp.Key.Y == this.previousLocation.Y).Key] = Symbol.Dot;
                        break;
                }

                inputs[0] = this.DetermineNextMoveDirection();

                counter++;
                if (counter == 2)
                {
                    counter = 0;
                    MapMaker MM = new MapMaker(this.Map.Keys.ToList(), Symbol.Empty);
                    //MM.PopulateRepairMap(this.Map);
                    //MM.PrintLiveUpdates();
                }
            }
        }

        public long GetShortestPath()
        {
            return this.ShortestPath(0, 0, 0, 0, 0);
        }

        public long GetLongestPath()
        {
            return this.LongestPath(this.TargetPosition.X, this.TargetPosition.Y, 0, this.TargetPosition.X, this.TargetPosition.Y);
        }

        private Symbol CheckForExisting(long moveDirection)
        {
            int xMod = 0;
            int yMod = 0;

            switch (moveDirection)
            {
                case 1: // north
                    xMod = 0;
                    yMod = -1;
                    break;
                case 2: // south
                    xMod = 0;
                    yMod = 1;
                    break;
                case 3: // west
                    xMod = -1;
                    yMod = 0;
                    break;
                case 4: // east
                    xMod = 1;
                    yMod = 0;
                    break;
            }

            if (this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + xMod && kvp.Key.Y == this.BotPosition.Y + yMod).Count() > 0) {
                return this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + xMod && kvp.Key.Y == this.BotPosition.Y + yMod).FirstOrDefault().Value;
            }

            return Symbol.Empty;
        }

        private void AddWall(long moveDirection)
        {
            switch (moveDirection)
            {
                case 1: // north
                    this.Map.Add(new Coordinate(this.BotPosition.X, this.BotPosition.Y-1), Symbol.Box);
                    break;
                case 2: // south
                    this.Map.Add(new Coordinate(this.BotPosition.X, this.BotPosition.Y+1), Symbol.Box);
                    break;
                case 3: // west
                    this.Map.Add(new Coordinate(this.BotPosition.X-1, this.BotPosition.Y), Symbol.Box);
                    break;
                case 4: // east
                    this.Map.Add(new Coordinate(this.BotPosition.X+1, this.BotPosition.Y), Symbol.Box);
                    break;
            }
        }

        private void UpdateBotPosition(long moveDirection)
        {
            switch (moveDirection)
            {
                case 1: // north
                    this.BotPosition.SetY(this.BotPosition.Y - 1);
                    break;
                case 2: // south
                    this.BotPosition.SetY(this.BotPosition.Y + 1);
                    break;
                case 3: // west
                    this.BotPosition.SetX(this.BotPosition.X - 1);
                    break;
                case 4: // east
                    this.BotPosition.SetX(this.BotPosition.X + 1);
                    break;
            }

            this.minX = this.BotPosition.X < this.minX ? this.BotPosition.X : this.minX;
            this.maxX = this.BotPosition.X > this.maxX ? this.BotPosition.X : this.maxX;
            this.minY = this.BotPosition.Y < this.minY ? this.BotPosition.Y : this.minY;
            this.maxY = this.BotPosition.Y > this.maxY ? this.BotPosition.Y : this.maxY;

            if (this.Map.Count > 100) // gross hack, but oh well
            {
                this.totalArea = (Math.Abs(this.minX) + Math.Abs(this.maxX) + 3) * (Math.Abs(this.minY) + Math.Abs(this.maxY) + 3);  // +3 are because the perimeter is always walls and we can't reach them, also account for the zero index
            }
        }

        private long DetermineNextMoveDirection()
        {
            int left = 1;
            int forward = 2;
            int right = 3;
            int back = 4;

            Dictionary<int, Coordinate> directionMods = new Dictionary<int, Coordinate>();
            Direction previousStep = this.Heading;

            switch (this.Heading)
            {
                case Direction.Up:
                    directionMods.Add(forward, new Coordinate(0, -1));
                    directionMods.Add(back, new Coordinate(0, 1));
                    directionMods.Add(left, new Coordinate(-1, 0));
                    directionMods.Add(right, new Coordinate(1, 0));
                    break;
                case Direction.Down:
                    directionMods.Add(forward, new Coordinate(0, 1));
                    directionMods.Add(back, new Coordinate(0, -1));
                    directionMods.Add(left, new Coordinate(1, 0));
                    directionMods.Add(right, new Coordinate(-1, 0));
                    break;
                case Direction.Left:
                    directionMods.Add(forward, new Coordinate(-1, 0));
                    directionMods.Add(back, new Coordinate(1, 0));
                    directionMods.Add(left, new Coordinate(0, 1));
                    directionMods.Add(right, new Coordinate(0, -1));
                    break;
                case Direction.Right:
                    directionMods.Add(forward, new Coordinate(1, 0));
                    directionMods.Add(back, new Coordinate(-1, 0));
                    directionMods.Add(left, new Coordinate(0, -1));
                    directionMods.Add(right, new Coordinate(0, 1));
                    break;
            }

            bool leftPath = true;
            bool rightPath = true;
            bool forwardPath = true;
            bool backPath = true;

            // check left path:
            if (this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).Count() > 0)
            {
                // left path has already been checked. Now remember if it's an available route even if we've been there:
                leftPath = this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).FirstOrDefault().Value != Symbol.Box;

                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).Key;
                if(temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Left;
                }
            }
            else // left path open, so take it
            {
                switch (this.Heading)
                {
                    case Direction.Left:
                        this.Heading = Direction.Down;
                        return 2;
                    case Direction.Up:
                        this.Heading = Direction.Left;
                        return 3;
                    case Direction.Right:
                        this.Heading = Direction.Up;
                        return 1;
                    case Direction.Down:
                        this.Heading = Direction.Right;
                        return 4;
                }
            }

            // now check forward:
            if (this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[forward].X && kvp.Key.Y == this.BotPosition.Y + directionMods[forward].Y).Count() > 0)
            {
                // forward path has already been checked. Now remember if it's an available route even if we've been there:
                forwardPath = this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[forward].X && kvp.Key.Y == this.BotPosition.Y + directionMods[forward].Y).FirstOrDefault().Value != Symbol.Box;
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[forward].X && kvp.Key.Y == this.BotPosition.Y + directionMods[forward].Y).Key;
                if (temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Forward;
                }
            }
            else // forward path open, so take it
            {
                switch (this.Heading)
                {
                    case Direction.Left:
                        return 3;
                    case Direction.Up:
                        return 1;
                    case Direction.Right:
                        return 4;
                    case Direction.Down:
                        return 2;
                }
            }

            // now check right:
            if (this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[right].X && kvp.Key.Y == this.BotPosition.Y + directionMods[right].Y).Count() > 0)
            {
                // right path has already been checked. Now remember if it's an available route even if we've been there:
                rightPath = this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[right].X && kvp.Key.Y == this.BotPosition.Y + directionMods[right].Y).FirstOrDefault().Value != Symbol.Box;
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[right].X && kvp.Key.Y == this.BotPosition.Y + directionMods[right].Y).Key;
                if (temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Right;
                }
            }
            else // right path open, so take it
            {
                switch (this.Heading)
                {
                    case Direction.Left:
                        this.Heading = Direction.Up;
                        return 1;
                    case Direction.Up:
                        this.Heading = Direction.Right;
                        return 4;
                    case Direction.Right:
                        this.Heading = Direction.Down;
                        return 2;
                    case Direction.Down:
                        this.Heading = Direction.Left;
                        return 3;
                }
            }

            // now check back:
            if (this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[back].X && kvp.Key.Y == this.BotPosition.Y + directionMods[back].Y).Count() > 0)
            {
                // back path has already been checked. Now remember if it's an available route even if we've been there:
                backPath = this.Map.Where(kvp => kvp.Key.X == this.BotPosition.X + directionMods[back].X && kvp.Key.Y == this.BotPosition.Y + directionMods[back].Y).FirstOrDefault().Value != Symbol.Box;
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[back].X && kvp.Key.Y == this.BotPosition.Y + directionMods[back].Y).Key;
                if (temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Back;
                }
            }
            else // back path open, so take it
            {
                switch (this.Heading)
                {
                    case Direction.Left:
                        this.Heading = Direction.Right;
                        return 4;
                    case Direction.Up:
                        this.Heading = Direction.Down;
                        return 2;
                    case Direction.Right:
                        this.Heading = Direction.Left;
                        return 3;
                    case Direction.Down:
                        this.Heading = Direction.Up;
                        return 1;
                }
            }

            Direction destination = Direction.Up;

            switch (this.Heading)
            {
                case Direction.Left:
                    destination = (leftPath && previousStep != Direction.Left) ? Direction.Down
                        : (forwardPath && previousStep != Direction.Forward) ? Direction.Left
                            : (rightPath && previousStep != Direction.Right) ? Direction.Up
                                : (backPath && previousStep != Direction.Back) ? Direction.Right
                                    : leftPath ? Direction.Down : forwardPath ? Direction.Left : rightPath ? Direction.Up : backPath ? Direction.Right : Direction.Left;
                    break;
                case Direction.Up:
                    destination = (leftPath && previousStep != Direction.Left) ? Direction.Left 
                        : (forwardPath && previousStep != Direction.Forward) ? Direction.Up 
                            : (rightPath && previousStep != Direction.Right) ? Direction.Right 
                                : (backPath && previousStep != Direction.Back) ? Direction.Down 
                                    : leftPath ? Direction.Left : forwardPath ? Direction.Up : rightPath ? Direction.Right : backPath ? Direction.Down : Direction.Up;
                    break;
                case Direction.Right:
                    destination = (leftPath && previousStep != Direction.Left) ? Direction.Up 
                        : (forwardPath && previousStep != Direction.Forward) ? Direction.Right 
                            : (rightPath && previousStep != Direction.Right) ? Direction.Down 
                                : (backPath && previousStep != Direction.Back) ? Direction.Left 
                                    : leftPath ? Direction.Up : forwardPath ? Direction.Right : rightPath ? Direction.Down : backPath ? Direction.Left : Direction.Right;
                    break;
                case Direction.Down:
                    destination = (leftPath && previousStep != Direction.Left) ? Direction.Right 
                        : (forwardPath && previousStep != Direction.Forward) ? Direction.Down 
                            : (rightPath && previousStep != Direction.Right) ? Direction.Left 
                                : (backPath && previousStep != Direction.Back) ? Direction.Up 
                                    : leftPath ? Direction.Right : forwardPath ? Direction.Down : rightPath ? Direction.Left : backPath ? Direction.Up : Direction.Down;
                    break;
            }

            this.Heading = destination;

            // If we've gotten here, all paths have been visited already, so take the first available from the left (that isn't the previous node)
            switch (destination)
            {
                case Direction.Left:
                    return 3;
                case Direction.Up:
                    return 1;
                case Direction.Right:
                    return 4;
                case Direction.Down:
                    return 2;
            }

            // won't ever get here
            return 0;
        }

        private long ShortestPath(int curX, int curY, long steps, int prevX, int prevY)
        {
            if(curX == this.TargetPosition.X && curY == this.TargetPosition.Y)
            {
                return steps;
            }

            //up
            if(curY - 1 != prevY && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX && kvp.Key.Y == curY - 1).Key] != Symbol.Box)
            {
                long temp = this.ShortestPath(curX, curY - 1, steps + 1, curX, curY);
                if(temp != 0)
                {
                    return temp;
                }
            }

            // down
            if (curY + 1 != prevY && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX && kvp.Key.Y == curY + 1).Key] != Symbol.Box)
            {
                long temp = this.ShortestPath(curX, curY + 1, steps + 1, curX, curY);
                if (temp != 0)
                {
                    return temp;
                }
            }

            // left
            if (curX - 1 != prevX && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX - 1 && kvp.Key.Y == curY).Key] != Symbol.Box)
            {
                long temp = this.ShortestPath(curX - 1, curY, steps + 1, curX, curY);
                if (temp != 0)
                {
                    return temp;
                }
            }

            // right
            if (curX + 1 != prevX && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX + 1 && kvp.Key.Y == curY).Key] != Symbol.Box)
            {
                long temp = this.ShortestPath(curX + 1, curY, steps + 1, curX, curY);
                if (temp != 0)
                {
                    return temp;
                }
            }

            return 0;
        }

        private long LongestPath(int curX, int curY, long steps, int prevX, int prevY)
        {
            long temp = steps;

            //up
            if (curY - 1 != prevY && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX && kvp.Key.Y == curY - 1).Key] != Symbol.Box)
            {
                temp = this.LongestPath(curX, curY - 1, steps + 1, curX, curY);
            }

            // down
            if (curY + 1 != prevY && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX && kvp.Key.Y == curY + 1).Key] != Symbol.Box)
            {
                long temp1 = this.LongestPath(curX, curY + 1, steps + 1, curX, curY);
                temp = temp1 > temp ? temp1 : temp;
            }

            // left
            if (curX - 1 != prevX && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX - 1 && kvp.Key.Y == curY).Key] != Symbol.Box)
            {
                long temp1 = this.LongestPath(curX - 1, curY, steps + 1, curX, curY);
                temp = temp1 > temp ? temp1 : temp;
            }

            // right
            if (curX + 1 != prevX && this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == curX + 1 && kvp.Key.Y == curY).Key] != Symbol.Box)
            {
                long temp1 = this.LongestPath(curX + 1, curY, steps + 1, curX, curY);
                temp = temp1 > temp ? temp1 : temp;
            }

            return temp;
        }
    }
}
