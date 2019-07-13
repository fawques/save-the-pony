using SaveThePony.Interfaces;
using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony.Services
{
    public class MazeWalker : IMazeWalker
    {
        IPonyAPIClient ponyClient;
        IMazeFactory factory;

        public MazeWalker(IPonyAPIClient ponyClient, IMazeFactory factory)
        {
            this.ponyClient = ponyClient;
            this.factory = factory;
        }

        public async Task Walk(Path path)
        {
            Point currentPosition = path.Source;
            var stepTuples = Enumerable.Zip(path.Steps, path.Steps.Skip(1), (next, secondNext) => (next, secondNext));
            Maze currentMaze = await factory.FromID(path.MazeId);
            foreach (var steps in stepTuples)
            {
                while (currentMaze.Domokun.Position.Equals(steps.next)
                    || currentMaze.GetTile(currentMaze.Domokun.Position).AccessibleTiles.Any(t => t.Equals(steps.next)))
                {
                    Console.WriteLine("Domokun is lurking near. Shhh");
                    currentMaze = await TakeStep(currentPosition, currentPosition, currentMaze);
                }
                currentMaze = await TakeStep(currentPosition, steps.next, currentMaze);
                currentPosition = steps.next;
            }

            currentMaze = await TakeStep(currentPosition, path.Steps.Last(), currentMaze);
        }

        async Task<Maze> TakeStep(Point currentPosition, Point next, Maze currentMaze)
        {
            string direction = GetDirection(currentPosition, next);
            if (direction != "stay")
            {
                Console.WriteLine($"Run pony! Run {direction}!");
            }
            await ponyClient.PostStep(currentMaze.MazeId, direction);
            Console.WriteLine(await ponyClient.GetVisualMaze(currentMaze.MazeId));
            return await factory.FromID(currentMaze.MazeId);
        }

        string GetDirection(Point current, Point next)
        {
            string direction;
            int xDifference = next.X - current.X;
            int yDifference = next.Y - current.Y;

            if (Math.Abs(xDifference) + Math.Abs(yDifference) > 1)
            {
                throw new InvalidOperationException("Steps must be 1 tile only");
            }

            if (xDifference == 0 && yDifference == 0)
            {
                direction = "stay";
            }
            else if (yDifference == 1)
            {
                direction = "south";
            }
            else if (yDifference == -1)
            {
                direction = "north";
            }
            else if (xDifference == 1)
            {
                direction = "east";
            }
            else
            {
                direction = "west";
            }

            return direction;
        }
    }
}
