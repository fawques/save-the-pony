using System;
using System.Collections.Generic;
using System.Text;

namespace SaveThePony.Models
{
    public class MazeObject
    {
        public MazeObject(int x, int y)
        {
            Position = new Point(x, y);
        }

        public Point Position { get; set; }
    }
}
