using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SaveThePony.Services
{
    public class Path
    {
        public Guid MazeId { get; set; }
        public Point Source { get; set; }
        public Point Destination { get; set; }
        public IEnumerable<Point> Steps { get; set; }
        public int Length => Steps.Count();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Path: ");
            sb.Append(Source);
            sb.Append("->");
            sb.AppendJoin(',', Steps);
            sb.Append("]");
            return sb.ToString();
        }
    }
}