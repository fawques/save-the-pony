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
            Mock<IPonyAPIClient> mockPonyAPI = new Mock<IPonyAPIClient>();
            pathPoster = new PathPoster(mockPonyAPI.Object);

            Path path = new Path
            {
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

            await pathPoster.Post(path);

            mockPonyAPI.Verify(api => api.PostStep(It.IsAny<Guid>(), It.IsAny<string>()), Times.Exactly(5));
        }
    }
}
