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
        private Coordinate previousLocation;
        private Direction Heading;
        private IntcodeComputer IC;

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
            bool mapComplete = false;
            previousLocation = this.Map.Keys.First();
            int counter = 0;

            while (!mapComplete)
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
                        if(this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == previousLocation.X && kvp.Key.Y == previousLocation.Y).Key] != Symbol.O)
                        {
                            this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == previousLocation.X && kvp.Key.Y == previousLocation.Y).Key] = Symbol.Dot;
                        }
                        previousLocation = newLocation;
                        break;
                    case 2: // target
                        this.UpdateBotPosition(inputs[0]);
                        Coordinate newLoc = new Coordinate(this.BotPosition.X, this.BotPosition.Y);
                        if (outputSymbol == Symbol.Empty)
                        {
                            this.Map.Add(newLoc, Symbol.O);
                        }
                        else
                        {
                            this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == newLoc.X && kvp.Key.Y == newLoc.Y).Key] = Symbol.O;
                        }
                        this.Map[this.Map.FirstOrDefault(kvp => kvp.Key.X == previousLocation.X && kvp.Key.Y == previousLocation.Y).Key] = Symbol.Dot;
                        previousLocation = newLoc;
                        break;
                }

                inputs[0] = this.DetermineNextMoveDirection();

                counter++;
                if (counter == 2)
                {
                    counter = 0;
                    MapMaker MM = new MapMaker(this.Map.Keys.ToList(), Symbol.Empty);
                    MM.PopulateRepairMap(this.Map);
                    MM.PrintLiveUpdates();
                    //MM.PrintWholeMap(true);
                }
            }
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

            bool leftPath = false;
            bool rightPath = false;
            bool forwardPath = false;
            bool backPath = false;

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
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).Key;
                if (temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Up;
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
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).Key;
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
                Coordinate temp = this.Map.FirstOrDefault(kvp => kvp.Key.X == this.BotPosition.X + directionMods[left].X && kvp.Key.Y == this.BotPosition.Y + directionMods[left].Y).Key;
                if (temp.X == this.previousLocation.X && temp.Y == this.previousLocation.Y)
                {
                    previousStep = Direction.Down;
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
                    destination = (leftPath && previousStep != Direction.Down) ? Direction.Down
                        : (forwardPath && previousStep != Direction.Left) ? Direction.Left
                            : (rightPath && previousStep != Direction.Up) ? Direction.Up
                                : (backPath && previousStep != Direction.Right) ? Direction.Right
                                    : leftPath ? Direction.Down : forwardPath ? Direction.Left : rightPath ? Direction.Up : backPath ? Direction.Right : Direction.Left;
                    break;
                case Direction.Up:
                    destination = (leftPath && previousStep != Direction.Left) ? Direction.Left 
                        : (forwardPath && previousStep != Direction.Up) ? Direction.Up 
                            : (rightPath && previousStep != Direction.Right) ? Direction.Right 
                                : (backPath && previousStep != Direction.Down) ? Direction.Down 
                                    : leftPath ? Direction.Left : forwardPath ? Direction.Up : rightPath ? Direction.Right : backPath ? Direction.Down : Direction.Up;
                    break;
                case Direction.Right:
                    destination = (leftPath && previousStep != Direction.Up) ? Direction.Up 
                        : (forwardPath && previousStep != Direction.Right) ? Direction.Right 
                            : (rightPath && previousStep != Direction.Down) ? Direction.Down 
                                : (backPath && previousStep != Direction.Left) ? Direction.Left 
                                    : leftPath ? Direction.Up : forwardPath ? Direction.Right : rightPath ? Direction.Down : backPath ? Direction.Left : Direction.Right;
                    break;
                case Direction.Down:
                    destination = (leftPath && previousStep != Direction.Right) ? Direction.Right 
                        : (forwardPath && previousStep != Direction.Down) ? Direction.Down 
                            : (rightPath && previousStep != Direction.Left) ? Direction.Left 
                                : (backPath && previousStep != Direction.Up) ? Direction.Up 
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
    }
}
