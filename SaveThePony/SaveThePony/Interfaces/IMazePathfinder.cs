using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaveThePony.Interfaces
{
    public interface IMazePathfinder
    {
        int CalculateManhattanDistance(Point source, Point destination);
        Path Solve(Maze maze);
    }
}
