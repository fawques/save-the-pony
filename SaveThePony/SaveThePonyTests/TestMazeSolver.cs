using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Models;
using SaveThePony.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StreamReader = System.IO.StreamReader;

namespace SaveThePonyTests
{
    public class TestMazeSolver
    {
        MazeSolver solver;

        string mazeJson;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var reader = new StreamReader(Utils.GetEmbeddedResourceFileStream("SaveThePonyTests.maze.json")))
            {
                mazeJson = reader.ReadToEnd();
            }
        }

        [Test]
        public void CalculateManhattanDistance_SimpleCase()
        {
            solver = new MazeSolver();

            Point source = new Point(10, 13);
            Point destination = new Point(24, 1);

            int distance = solver.CalculateManhattanDistance(source, destination);

            Assert.AreEqual(26, distance);
        }

        [Test]
        public void GetShortestPath_MazeWithoutWalls_PathIsManhattanDistance()
        {
            Maze maze = CreateMaze();
            solver = new MazeSolver();

            Path path = solver.Solve(maze);


            Assert.AreEqual(29, path.Length); // MD == 28 + start
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.Pony.Position, path.Steps.First());
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }

        [Test]
        public void GetShortestPath_SmallMaze_PathIsShortest()
        {
            Maze maze = CreateRectangularMaze();
            solver = new MazeSolver();

            Path path = solver.Solve(maze);

            Path expectedPath = new Path { Source = maze.Pony.Position, Destination = maze.EndPoint };
            expectedPath.Steps = new List<Point>
            {
                new Point(1,2),
                new Point(2,2),
                new Point(3,2),
                new Point(3,3),
                new Point(3,4),
                new Point(2,4),
                new Point(1,4),
                new Point(0,4),
                new Point(0,5),
                new Point(0,6),
                new Point(0,7),
                new Point(0,8),
                new Point(1,8),
                new Point(2,8)
            };

            Assert.AreEqual(14, path.Length); // MD == 28 + start
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.Pony.Position, path.Steps.First());
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
            Assert.AreEqual(expectedPath.Steps, path.Steps);
        }

        [Test]
        public void GetShortestPath_RealMazeNoMonster_PathIsShortest()
        {
            MazeFactory factory = new MazeFactory(Mock.Of<IPonyAPIClient>());
            Maze maze = factory.FromJson(mazeJson);
            maze.Domokun.Position = new Point(-1, -1);
            solver = new MazeSolver();


            Path path = solver.Solve(maze);

            Path expectedPath = new Path { Source = maze.Pony.Position, Destination = maze.EndPoint };
            expectedPath.Steps = new List<Point>
            {
                new Point(5,4),
                new Point(4,4),
                new Point(4,3),
                new Point(4,2),
                new Point(5,2),
                new Point(6,2),
                new Point(6,1),
                new Point(5,1),
                new Point(4,1),
                new Point(4,0),
                new Point(3,0),
                new Point(3,1),
            };

            Assert.AreEqual(expectedPath.Steps, path.Steps.Take(12));
            Assert.AreEqual(82, path.Length);
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.Pony.Position, path.Steps.First());
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }

        Maze CreateMaze()
        {
            Maze maze = new Maze
            {
                Width = 15,
                Height = 15,
                Difficulty = 0,
                Tiles = new MazeTile[15, 15],
                Pony = new Pony(0, 0),
                EndPoint = new Point(14, 14)
            };

            for (int row = 0; row < maze.Height; row++)
            {
                for (int column = 0; column < maze.Width; column++)
                {

                    MazeTile tile = new MazeTile(column, row);
                    maze.Tiles[column, row] = tile;

                    if (tile.Position.X != 0)
                    {
                        tile.AccessibleTiles.Add(new Point(tile.Position.X - 1, tile.Position.Y));
                    }
                    if (tile.Position.Y != 0)
                    {
                        tile.AccessibleTiles.Add(new Point(tile.Position.X, tile.Position.Y - 1));
                    }
                    if (tile.Position.X != maze.Width - 1)
                    {
                        tile.AccessibleTiles.Add(new Point(tile.Position.X + 1, tile.Position.Y));
                    }
                    if (tile.Position.Y != maze.Height - 1)
                    {
                        tile.AccessibleTiles.Add(new Point(tile.Position.X, tile.Position.Y + 1));
                    }
                }
            }
            return maze;
        }

        Maze CreateRectangularMaze()
        {
            Maze maze = new Maze
            {
                Width = 4,
                Height = 10,
                Difficulty = 0,
                Tiles = new MazeTile[4, 10],
                Pony = new Pony(1, 2),
                EndPoint = new Point(2, 8)
            };

            List<Point> walls = new List<Point> {
                new Point(0,3),
                new Point(1,3),
                new Point(2,3),
                new Point(1,5),
                new Point(2,5),
                new Point(3,5)
            };

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

        void AddAccessible(List<Point> walls, MazeTile tile, Point accesible)
        {
            if (!walls.Contains(accesible))
            {
                tile.AccessibleTiles.Add(accesible);
            }
        }
    }
}
