using Moq;
using NUnit.Framework;
using SaveThePony;
using SaveThePony.Models;
using SaveThePony.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePonyTests
{
    public class TestPathPoster
    {
        PathPoster pathPoster;

        [Test]
        public async Task Post_SingleLine_SuccessfullyPosted()
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

            var directions = new List<string>();
            mockPonyAPI.Setup(m => m.PostStep(mazeId, Capture.In(directions)));

            pathPoster = new PathPoster(mockPonyAPI.Object);

            await pathPoster.Post(path);

            Assert.AreEqual(new[] { "south", "south", "south", "south", "south" }, directions);
        }

        [Test]
        public async Task Post_AllPossibleDirections_SuccessfullyPosted()
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

            var directions = new List<string>();
            mockPonyAPI.Setup(m => m.PostStep(mazeId, Capture.In(directions)));

            pathPoster = new PathPoster(mockPonyAPI.Object);

            await pathPoster.Post(path);

            Assert.AreEqual(new[] { "south", "east", "north", "west", "stay" }, directions);
        }
    }
}
