using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Models;
using SaveThePony.Services;
using System.Collections.Generic;
using System.Linq;
using StreamReader = System.IO.StreamReader;

namespace SaveThePonyTests
{
    public class TestMazeSolver
    {
        MazeSolver solver;

        string mazeJson;
        readonly Point OUT_OF_BOUNDS = new Point(-1, -1);

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
            Maze maze = CreateMaze(width: 15,
                height: 15,
                difficulty: 0,
                ponyPosition: new Point(0, 0),
                endPoint: new Point(14, 14));
            solver = new MazeSolver();

            Path path = solver.Solve(maze);


            Assert.AreEqual(28, path.Length); // MD == 28 + start
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }

        [Test]
        public void GetShortestPath_SmallMaze_PathIsShortest()
        {
            Maze maze = CreateMaze(width: 4,
                height: 10,
                difficulty: 0,
                ponyPosition: new Point(1, 2),
                endPoint: new Point(2, 8),
                monsterPosition: null,
                walls: new List<Point> {
                    new Point(0,3),
                    new Point(1,3),
                    new Point(2,3),
                    new Point(1,5),
                    new Point(2,5),
                    new Point(3,5)
                });
            solver = new MazeSolver();

            Path path = solver.Solve(maze);

            Path expectedPath = new Path { Source = maze.Pony.Position, Destination = maze.EndPoint };
            expectedPath.Steps = new List<Point>
            {
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

            Assert.AreEqual(13, path.Length);
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
            Assert.AreEqual(expectedPath.Steps, path.Steps);
        }

        [Test]
        public void GetShortestPath_NoMonster_PathIsShortest()
        {
            MazeFactory factory = new MazeFactory(Mock.Of<IPonyAPIClient>());
            Maze maze = factory.FromJson(mazeJson);
            maze.Domokun.Position = new Point(-1, -1);
            solver = new MazeSolver();


            Path path = solver.Solve(maze);

            Path expectedPath = new Path { Source = maze.Pony.Position, Destination = maze.EndPoint };
            expectedPath.Steps = new List<Point>
            {
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

            Assert.AreEqual(expectedPath.Steps, path.Steps.Take(11));
            Assert.AreEqual(81, path.Length);
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }

        [Test]
        public void GetShortestPath_MonsterInPath_NoPossiblePath()
        {
            MazeFactory factory = new MazeFactory(Mock.Of<IPonyAPIClient>());
            Maze maze = factory.FromJson(mazeJson);
            // 4,0 is in the middle of the path, at position 9
            maze.Domokun.Position = new Point(4, 0);
            solver = new MazeSolver();

            Path path = solver.Solve(maze);

            Assert.AreEqual(0, path.Length);
        }

        [Test]
        public void GetShortestPath_MonsterTooNear_NoPossiblePath()
        {
            MazeFactory factory = new MazeFactory(Mock.Of<IPonyAPIClient>());
            Maze maze = factory.FromJson(mazeJson);
            // This is seven tiles away of the endpoint, from there the paths collide
            maze.Domokun.Position = new Point(9, 14);
            solver = new MazeSolver();

            Path path = solver.Solve(maze);

            Assert.AreEqual(0, path.Length);
        }

        [Test]
        public void GetShortestPath_MonsterTooFarAway_PathIsShortest()
        {
            MazeFactory factory = new MazeFactory(Mock.Of<IPonyAPIClient>());
            Maze maze = factory.FromJson(mazeJson);
            // This behind the pony, so the monster will never catch the pony
            maze.Domokun.Position = new Point(5, 3);
            solver = new MazeSolver();

            Path path = solver.Solve(maze);

            Path expectedPath = new Path { Source = maze.Pony.Position, Destination = maze.EndPoint };
            expectedPath.Steps = new List<Point>
            {
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

            Assert.AreEqual(expectedPath.Steps, path.Steps.Take(11));
            Assert.AreEqual(81, path.Length);
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }

        Maze CreateMaze(int width, int height, int difficulty, Point ponyPosition, Point endPoint, Point monsterPosition = null, List<Point> walls = null)
        {
            Maze maze = new Maze
            {
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

        void AddAccessible(List<Point> walls, MazeTile tile, Point accesible)
        {
            if (!walls.Contains(accesible))
            {
                tile.AccessibleTiles.Add(accesible);
            }
        }
    }
}
