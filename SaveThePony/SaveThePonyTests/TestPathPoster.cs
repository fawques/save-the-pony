using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Interfaces;
using SaveThePony.Models;
using SaveThePony.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SaveThePonyTests.Utils;

namespace SaveThePonyTests
{
    public class TestPathPoster
    {
        MazeWalker mazeWalker;

        [Test]
        public async Task Walk_SingleLine_SuccessfullyPosted()
        {
            Guid mazeId = Guid.NewGuid();
            Path path = new Path
            {
                MazeId = mazeId,
                Source = new Point(0, 0),
                Destination = new Point(0, 5),
                Steps = new List<Point>
                {
                    new Point(0, 1),
                    new Point(0, 2),
                    new Point(0, 3),
                    new Point(0, 4),
                    new Point(0, 5),
                }
            };

            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            Mock<IMazeFactory> mockMazeFactory = new Mock<IMazeFactory>();

            var directions = new List<string>();
            mockPonyAPI.Setup(m => m.PostStep(mazeId, Capture.In(directions)));
            Maze maze = CreateMaze(width: 15,
                height: 15,
                difficulty: 10,
                ponyPosition: new Point(-1, -1),
                endPoint: path.Destination,
                monsterPosition: new Point(10, 10)
            );
            maze.MazeId = mazeId;
            mockMazeFactory.Setup(m => m.FromID(mazeId)).ReturnsAsync(maze);

            mazeWalker = new MazeWalker(mockPonyAPI.Object, mockMazeFactory.Object);

            await mazeWalker.Walk(path);

            Assert.AreEqual(new[] { "south", "south", "south", "south", "south" }, directions);
        }

        [Test]
        public async Task Walk_AllPossibleDirections_SuccessfullyPosted()
        {
            Guid mazeId = Guid.NewGuid();
            Path path = new Path
            {
                MazeId = mazeId,
                Source = new Point(3, 3),
                Destination = new Point(3, 4),
                Steps = new List<Point>
                {
                    new Point(3, 4),
                    new Point(4, 4),
                    new Point(4, 3),
                    new Point(3, 3),
                    new Point(3, 3),
                }
            };

            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            Mock<IMazeFactory> mockMazeFactory = new Mock<IMazeFactory>();

            var directions = new List<string>();
            mockPonyAPI.Setup(m => m.PostStep(mazeId, Capture.In(directions)));
            Maze maze = CreateMaze(width: 15,
                height: 15,
                difficulty: 10,
                ponyPosition: new Point(-1, -1),
                endPoint: path.Destination,
                monsterPosition: new Point(10, 10)
            );
            maze.MazeId = mazeId;
            mockMazeFactory.Setup(m => m.FromID(mazeId)).ReturnsAsync(maze);

            mazeWalker = new MazeWalker(mockPonyAPI.Object, mockMazeFactory.Object);

            await mazeWalker.Walk(path);

            Assert.AreEqual(new[] { "south", "east", "north", "west", "stay" }, directions);
        }

        [Test]
        public async Task Walk_WithMonster_MonsterAvoided()
        {
            // Arrange
            /* Maze with Endpoint and Monster with this positions: 
             * ----|D
             * P
             * ----
             *     |E
             *     
             * For the test, the monster will only move when P is about to cross.
             */
            Maze maze = CreateMaze(width: 15,
                height: 15,
                difficulty: 10,
                ponyPosition: new Point(1, 2),
                endPoint: new Point(5, 3),
                monsterPosition: new Point(5, 1),
                walls: new List<Point>
                {
                    new Point(2,1),
                    new Point(2,3),
                    new Point(3,1),
                    new Point(3,3),
                    new Point(4,1),
                    new Point(4,3),
                    new Point(5,0),
                    new Point(5,4),
                }
            );
            Path path = new Path
            {
                MazeId = maze.MazeId,
                Source = maze.Pony.Position,
                Destination = maze.EndPoint,
                Steps = new List<Point>
                {
                    new Point(2, 2),
                    new Point(3, 2),
                    new Point(4, 2),
                    new Point(5, 2),
                    new Point(5, 3),
                }
            };

            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            Point monsterRightOfPony = new Point(maze.Domokun.Position.X, maze.Domokun.Position.Y + 1);
            Point monsterOnTopOfPony = new Point(maze.Domokun.Position.X - 1, maze.Domokun.Position.Y + 1);
            Point monsterLeftOfPony = new Point(maze.Domokun.Position.X - 2, maze.Domokun.Position.Y + 1);
            MovingMonsterMazeFactory mazeFactory = new MovingMonsterMazeFactory(maze, new List<Point>
            {
                maze.Domokun.Position,
                maze.Domokun.Position,
                maze.Domokun.Position,
                maze.Domokun.Position,
                // Domokun starts walking
                monsterRightOfPony,
                monsterOnTopOfPony,
                // Domokun goes back, pony shoud wait more
                monsterRightOfPony,
                monsterOnTopOfPony,
                // Domokun goes past the pony
                monsterLeftOfPony,
                monsterLeftOfPony,
                monsterLeftOfPony,
            });

            var directions = new List<string>();
            mockPonyAPI.Setup(m => m.PostStep(maze.MazeId, Capture.In(directions)));

            mazeWalker = new MazeWalker(mockPonyAPI.Object, mazeFactory);

            // Act
            await mazeWalker.Walk(path);

            // Assert
            Assert.AreEqual(new[] { "east", "east", "east", "stay", "stay", "stay", "stay", "stay", "east", "south" }, directions);
        }

        class MovingMonsterMazeFactory : IMazeFactory
        {
            int count;
            Maze maze;
            List<Point> monsterPositions;

            public MovingMonsterMazeFactory(Maze maze, List<Point> monsterPositions)
            {
                this.monsterPositions = monsterPositions;
                this.maze = maze;
                count = 0;
            }

            public Task<Maze> Create(int width, int height, string ponyName, int difficulty)
            {
                throw new NotImplementedException();
            }

            public Task<Maze> Create()
            {
                throw new NotImplementedException();
            }

            public Task<Maze> FromID(Guid mapId)
            {
                maze.Domokun.Position = monsterPositions[count];
                count++;
                return Task.FromResult(maze);
            }

            public Maze FromJson(string jsonMaze)
            {
                throw new NotImplementedException();
            }
        }
    }
}
