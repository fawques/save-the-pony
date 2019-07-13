using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SaveThePony.Models
{
    public class Maze
    {
        public Guid MazeId { get; set; }
        public MazeTile[,] Tiles { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public Pony Pony { get; set; }
        public Monster Domokun { get; set; }
        public Point EndPoint { get; set; }
        public int Difficulty { get; set; }

        public MazeTile GetTile(Point position)
        {
            return Tiles[position.X, position.Y];
        }
    }
}
