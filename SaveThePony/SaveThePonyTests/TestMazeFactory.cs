using NUnit.Framework;
using SaveThePony;
using System;

namespace Tests
{
    public class TestMazeFactory
    {
        MazeFactory mazeFactory;

        [Test]
        public void BuildMaze_FromExistingID_ApiCalled()
        {
            Guid mazeId = Guid.NewGuid();
            mazeFactory = new MazeFactory();



            Maze maze = mazeFactory.FromID(mazeId);

            Assert.IsNotNull(maze);
        }
    }
}