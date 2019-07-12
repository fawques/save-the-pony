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
            MockSequence mockSequence = new MockSequence();

            mockPonyAPI.InSequence(mockSequence).Setup(api => api.PostStep(mazeId, "south"));
            mockPonyAPI.InSequence(mockSequence).Setup(api => api.PostStep(mazeId, "south"));
            mockPonyAPI.InSequence(mockSequence).Setup(api => api.PostStep(mazeId, "south"));
            mockPonyAPI.InSequence(mockSequence).Setup(api => api.PostStep(mazeId, "south"));
            mockPonyAPI.InSequence(mockSequence).Setup(api => api.PostStep(mazeId, "south"));

            pathPoster = new PathPoster(mockPonyAPI.Object);

            await pathPoster.Post(path);

            mockPonyAPI.Verify(api => api.PostStep(mazeId, "down"), Times.Exactly(5));
        }
    }
}
