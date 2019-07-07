using System;
using System.Collections.Generic;
using System.Text;

namespace SaveThePony.Models
{
    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Point pointObj)
            {
                return pointObj.X == X && pointObj.Y == Y;
            }
            return base.Equals(obj);
        }
    }
}
