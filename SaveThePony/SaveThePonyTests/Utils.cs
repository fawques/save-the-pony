using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SaveThePonyTests
{
    public static class Utils
    {
        readonly static Point OUT_OF_BOUNDS = new Point(-1, -1);
        public static Stream GetEmbeddedResourceFileStream(string resourceId)
        {
            var assembly = Assembly.GetCallingAssembly();
            if (assembly.GetManifestResourceNames().Contains(resourceId))
            {
                return assembly.GetManifestResourceStream(resourceId);
            }
            return null;
        }



        public static Maze CreateMaze(int width, int height, int difficulty, Point ponyPosition, Point endPoint, Point monsterPosition = null, List<Point> walls = null)
        {
            Maze maze = new Maze
            {
                MazeId = Guid.NewGuid(),
                Width = width,
                Height = height,
                Difficulty = difficulty,
                Tiles = new MazeTile[width, height],
                Pony = new Pony(ponyPosition),
                EndPoint = endPoint,
                Domokun = new Monster(monsterPosition ?? OUT_OF_BOUNDS)
            };

            if (walls is null)
            {
                walls = new List<Point>();

            }

            for (int row = 0; row < maze.Height; row++)
            {
                for (int column = 0; column < maze.Width; column++)
                {

                    MazeTile tile = new MazeTile(column, row);
                    maze.Tiles[column, row] = tile;

                    if (tile.Position.X != 0)
                    {
                        Point accesible = new Point(tile.Position.X - 1, tile.Position.Y);
                        AddAccessible(walls, tile, accesible);
                    }
                    if (tile.Position.Y != 0)
                    {
                        Point accesible = new Point(tile.Position.X, tile.Position.Y - 1);
                        AddAccessible(walls, tile, accesible);
                    }
                    if (tile.Position.X != maze.Width - 1)
                    {
                        Point accesible = new Point(tile.Position.X + 1, tile.Position.Y);
                        AddAccessible(walls, tile, accesible);
                    }
                    if (tile.Position.Y != maze.Height - 1)
                    {
                        Point accesible = new Point(tile.Position.X, tile.Position.Y + 1);
                        AddAccessible(walls, tile, accesible);
                    }
                }
            }
            return maze;
        }

        static void AddAccessible(List<Point> walls, MazeTile tile, Point accesible)
        {
            if (!walls.Contains(accesible))
            {
                tile.AccessibleTiles.Add(accesible);
            }
        }
    }
}
