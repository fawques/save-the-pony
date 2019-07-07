using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveThePony.Models;
using SaveThePony.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony
{
    public class MazeFactory
    {
        IPonyAPIClient ponyClient;

        public MazeFactory(IPonyAPIClient ponyClient)
        {
            this.ponyClient = ponyClient;
        }

        public async Task<Maze> Create(int width, int height, string ponyName, int difficulty)
        {
            var response = await ponyClient.CreateMaze(width, height, ponyName, difficulty);
            var creationDefinition = new { maze_id = Guid.Empty };
            var creationResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), creationDefinition);

            return await FromID(creationResponse.maze_id);
        }

        public async Task<Maze> FromID(Guid mapId)
        {
            var response = await ponyClient.GetMaze(mapId);
            var apiMaze = JsonConvert.DeserializeObject<APIMaze>(await response.Content.ReadAsStringAsync());
            return BuildMaze(apiMaze);
        }

        Maze BuildMaze(APIMaze apiMaze)
        {
            int width = apiMaze.size[0];
            int height = apiMaze.size[1];
            MazeTile[,] tiles = new MazeTile[width, height];
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {

                    MazeTile mazeTile = new MazeTile(column, row);
                    mazeTile.AccessibleTiles = new List<Point>();
                    tiles[column, row] = mazeTile;
                    CalculateAccessibilities(apiMaze, width, height, row, column, mazeTile);
                }
            }

            int ponyX = apiMaze.pony[0] / width;
            int ponyY = apiMaze.pony[0] % width;

            int domokunX = apiMaze.domokun[0] / width;
            int domokunY = apiMaze.domokun[0] % width;

            var maze = new Maze
            {
                Width = width,
                Height = height,
                Difficulty = apiMaze.difficulty,
                Tiles = tiles,
                Pony = new Pony { X = ponyX, Y = ponyY },
                Domokun = new Monster { X = domokunX, Y = domokunY }
            };
            return maze;
        }

        int GetIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        void CalculateAccessibilities(APIMaze apiMaze, int width, int height, int row, int column, MazeTile mazeTile)
        {
            int currentIndex = GetIndex(column, row, width);
            string[] currentData = apiMaze.data[currentIndex];
            string[] eastData = new string[] { };
            string[] southData = new string[] { };
            if (column + 1 < width)
            {
                eastData = apiMaze.data[currentIndex + 1];
            }
            if (row + 1 < height)
            {
                southData = apiMaze.data[currentIndex + width];
            }
            CalculateAccesiblePoints(mazeTile, currentData, eastData, southData);
        }

        void CalculateAccesiblePoints(MazeTile mazeTile, string[] currentData, string[] eastData, string[] southdata)
        {
            /* Accessible tiles can be:
             * north - if currentData doesn't contain "north"
             * west - if currentData doesn't contain "west"
             * east - if data + (1,0) doesn't contain "west"
             * south - if data + (0,width) doesn't contain "north"
             * 
             */
            if (!currentData.Contains("north"))
            {
                mazeTile.AccessibleTiles.Add(new Point(mazeTile.Position.X, mazeTile.Position.Y - 1));
            }
            if (!currentData.Contains("west"))
            {
                mazeTile.AccessibleTiles.Add(new Point(mazeTile.Position.X - 1, mazeTile.Position.Y));
            }
            if (!southdata.Contains("north"))
            {
                mazeTile.AccessibleTiles.Add(new Point(mazeTile.Position.X, mazeTile.Position.Y + 1));
            }
            if (!eastData.Contains("west"))
            {
                mazeTile.AccessibleTiles.Add(new Point(mazeTile.Position.X + 1, mazeTile.Position.Y));
            }
        }
    }
}
