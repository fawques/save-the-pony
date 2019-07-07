using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Models;
using SaveThePonyTests;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class TestMazeFactory
    {
        MazeFactory mazeFactory;

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
        public async Task BuildMaze_New_ApiCalled()
        {
            Guid mazeId = Guid.NewGuid();
            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            mazeFactory = new MazeFactory(mockPonyAPI.Object);
            int width = 15;
            int height = 15;
            int difficulty = 5;
            string pony = "PonyName";

            mockPonyAPI.Setup(p => p.CreateMaze(width, height, pony, difficulty)).ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"maze_id\": \"" + mazeId + "\"}", Encoding.UTF8, "application/json")
                });
            mockPonyAPI.Setup(p => p.GetMaze(mazeId)).ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(mazeJson, Encoding.UTF8, "application/json")
                });

            Maze maze = await mazeFactory.Create(width, height, pony, difficulty);

            mockPonyAPI.Verify(p => p.CreateMaze(width, height, pony, difficulty), Times.Once);
            mockPonyAPI.Verify(p => p.GetMaze(mazeId), Times.Once);
            Assert.AreEqual(width, maze.Width);
            Assert.AreEqual(height, maze.Height);
        }

        [Test]
        public async Task BuildMaze_FromExistingID_ApiCalled()
        {
            Guid mazeId = Guid.NewGuid();
            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            mazeFactory = new MazeFactory(mockPonyAPI.Object);
            mockPonyAPI.Setup(p => p.GetMaze(mazeId)).ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(mazeJson, Encoding.UTF8, "application/json")
                });

            Maze maze = await mazeFactory.FromID(mazeId);

            mockPonyAPI.Verify(p => p.GetMaze(mazeId), Times.Once);
        }

        [Test]
        public async Task BuildMaze_FromJson_MazeCreatedCorrectly()
        {
            Guid mazeId = Guid.NewGuid();
            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            mockPonyAPI.Setup(p => p.GetMaze(mazeId)).ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(mazeJson, Encoding.UTF8, "application/json")
                });

            mazeFactory = new MazeFactory(mockPonyAPI.Object);

            Maze maze = await mazeFactory.FromID(mazeId);

            mockPonyAPI.Verify(p => p.GetMaze(mazeId), Times.Once);
            Assert.IsNotNull(maze);
            Assert.AreEqual(15, maze.Width);
            Assert.AreEqual(15, maze.Height);
            Assert.AreEqual(15 * 15, maze.Tiles.Length);
            Assert.AreEqual(4, maze.Pony.X);
            Assert.AreEqual(5, maze.Pony.Y);
            Assert.AreEqual(4, maze.Domokun.X);
            Assert.AreEqual(10, maze.Domokun.Y);
            Assert.AreEqual(new Point(0, 1), maze.Tiles[0, 0].AccessibleTiles.First());
            Assert.Contains(new Point(2, 0), maze.Tiles[3, 0].AccessibleTiles);
            Assert.Contains(new Point(4, 0), maze.Tiles[3, 0].AccessibleTiles);
            Assert.Contains(new Point(3, 1), maze.Tiles[3, 0].AccessibleTiles);
            Assert.Contains(new Point(0, 0), maze.Tiles[0, 1].AccessibleTiles);
        }
    }
}