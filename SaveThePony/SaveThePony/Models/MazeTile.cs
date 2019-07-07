using System.Collections.Generic;

namespace SaveThePony.Models
{
    public class MazeTile
    {
        public MazeTile(int x, int y)
        {
            Position = new Point(x, y);
            AccessibleTiles = new List<Point>();
        }

        public Point Position { get; set; }

        public List<Point> AccessibleTiles { get; set; }
    }
}