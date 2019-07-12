using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony.Services
{
    public class PathPoster
    {
        IPonyAPIClient ponyClient;

        public PathPoster(IPonyAPIClient ponyClient)
        {
            this.ponyClient = ponyClient;
        }

        public async Task Post(Path path)
        {
            Point current = path.Source;
            foreach (var step in path.Steps)
            {
                string direction = "";
                int xDifference = step.X - current.X;
                int yDifference = step.Y - current.Y;

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
                await ponyClient.PostStep(path.MazeId, direction);
                current = step;
            }
        }
    }
}
