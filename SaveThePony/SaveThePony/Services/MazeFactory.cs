using Newtonsoft.Json;
using SaveThePony.Interfaces;
using SaveThePony.Models;
using SaveThePony.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaveThePony.Services
{
    public class MazeFactory : IMazeFactory
    {
        IPonyAPIClient ponyClient;

        public MazeFactory(IPonyAPIClient ponyClient)
        {
            this.ponyClient = ponyClient;
        }

        public async Task<Maze> Create()
        {
            Console.WriteLine("Creating new maze");
            Console.WriteLine("Enter width:");
            int width = GetNumber(15, 25);
            Console.WriteLine("Enter height:");
            int height = GetNumber(15, 25);
            Console.WriteLine("Enter difficulty:");
            int difficulty = GetNumber(0, 10);
            Console.WriteLine("Enter pony name:");
            string ponyName = GetPonyName();
            return await Create(width, height, ponyName, difficulty);
        }

        int GetNumber(int min, int max)
        {
            int size;
            while (!int.TryParse(Console.ReadLine(), out size) || size < min || size > max)
            {
                Console.WriteLine($"Must be a number between {min} and {max}");
            }
            return size;
        }

        string GetPonyName()
        {
            List<string> validPonyNames = new List<string>
            {
                "Twilight Sparkle",
                "Applejack",
                "Fluttershy",
                "Rarity",
                "Pinkie Pie",
                "Rainbow Dash",
                "Spike",
            };
            string name = Console.ReadLine().Trim();
            while (!validPonyNames.Contains(name))
            {
                Console.WriteLine("Must be a valid pony name");
                name = Console.ReadLine().Trim();
            }
            return name;
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
            string jsonMaze = await response.Content.ReadAsStringAsync();
            var maze = FromJson(jsonMaze);
            maze.MazeId = mapId;
            return maze;
        }

        public Maze FromJson(string jsonMaze)
        {
            var apiMaze = JsonConvert.DeserializeObject<APIMaze>(jsonMaze);
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

            int ponyY = apiMaze.pony[0] / width;
            int ponyX = apiMaze.pony[0] % width;

            int domokunY = apiMaze.domokun[0] / width;
            int domokunX = apiMaze.domokun[0] % width;

            int endpointY = apiMaze.endPont[0] / width;
            int endpointX = apiMaze.endPont[0] % width;

            var maze = new Maze
            {
                Width = width,
                Height = height,
                Difficulty = apiMaze.difficulty,
                Tiles = tiles,
                Pony = new Pony(ponyX, ponyY),
                Domokun = new Monster(domokunX, domokunY),
                EndPoint = new Point(endpointX, endpointY)
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
            else if (column + 1 == width)
            {
                eastData = new string[] { "west" };
            }

            if (row + 1 < height)
            {
                southData = apiMaze.data[currentIndex + width];
            }
            else if (row + 1 == height)
            {
                southData = new string[] { "north" };
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
