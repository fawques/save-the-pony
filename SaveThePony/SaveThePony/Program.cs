using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaveThePony.Models;
using SaveThePony.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaveThePony
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            //collection.AddScoped<IDemoService, DemoService>();
            // ...
            // Add other services
            // ...
            serviceCollection.AddScoped<IPonyAPIClient, PonyAPIClient>();

            Configure(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            var ponyAPI = services.GetRequiredService<IPonyAPIClient>();
            MazeFactory mazeFactory = new MazeFactory(ponyAPI);

            Maze maze = null;
            while (maze is null)
            {
                Console.WriteLine("Enter a maze GUID or press enter for a new maze:");
                string input = Console.ReadLine();
                if (input != "")
                {
                    if (Guid.TryParse(input, out Guid mazeId))
                    {
                        maze = await mazeFactory.FromID(mazeId);
                    }
                    else
                    {
                        Console.WriteLine("Could not parse mazeId");
                    }
                }
                else
                {
                    Console.WriteLine("Creating new maze");
                    Console.WriteLine("Enter width:");
                    int width = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter height:");
                    int height = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter difficulty:");
                    int difficulty = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter pony name:");
                    string ponyName = Console.ReadLine().Trim();

                    maze = await mazeFactory.Create(width, height, ponyName, difficulty);
                    Console.WriteLine($"Maze ID {maze.MazeId} created.");

                }
            }
            var mazeVisual = await ponyAPI.GetVisualMaze(maze.MazeId);
            Console.WriteLine(mazeVisual);

            MazeSolver solver = new MazeSolver();
            Path ponyPath = solver.Solve(maze);
            if (ponyPath.Length == 0)
            {
                Console.WriteLine("The little pony was so mesmerized with the rainbow lights that she is crossing paths with Domokun. Let's hope he is distracted too...");
                // TODO: Post even if Domo
            }
            else
            {
                Console.WriteLine("The little pony got out! Here's the path:");
                Console.WriteLine(ponyPath);
                PathPoster pathPoster = new PathPoster(ponyAPI);
                await pathPoster.Post(ponyPath);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            services.Dispose();
        }

        public static void Configure(IServiceCollection services)
        {
            services.AddHttpClient<IPonyAPIClient, PonyAPIClient>();
        }

    }
}
