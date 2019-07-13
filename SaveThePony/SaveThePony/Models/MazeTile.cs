using System.Collections.Generic;

namespace SaveThePony.Models
{
    public class MazeTile : MazeObject
    {
        public MazeTile(int x, int y) : base(x, y)
        {
            AccessibleTiles = new List<Point>();
        }

        public List<Point> AccessibleTiles { get; set; }
    }
}