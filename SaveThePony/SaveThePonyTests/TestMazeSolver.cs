using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Models;
using SaveThePony.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using StreamReader = System.IO.StreamReader;
using static SaveThePonyTests.Utils;

namespace SaveThePonyTests
{
    public class TestMazeSolver
    {
        MazePathfinder solver;

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
            solver = new MazePathfinder();

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
            solver = new MazePathfinder();

            Path path = solver.Solve(maze);


            Assert.AreEqual(maze.MazeId, path.MazeId);
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
            solver = new MazePathfinder();

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

            Assert.AreEqual(maze.MazeId, path.MazeId);
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
            solver = new MazePathfinder();


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

            Assert.AreEqual(maze.MazeId, path.MazeId);
            Assert.AreEqual(expectedPath.Steps, path.Steps.Take(11));
            Assert.AreEqual(81, path.Length);
            Assert.AreEqual(maze.Pony.Position, path.Source);
            Assert.AreEqual(maze.EndPoint, path.Destination);
            Assert.AreEqual(maze.EndPoint, path.Steps.Last());
        }
    }
}
