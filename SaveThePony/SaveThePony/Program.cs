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
                    maze = await mazeFactory.Create();
                    Console.WriteLine($"Maze ID {maze.MazeId} created.");

                }
            }
            var mazeVisual = await ponyAPI.GetVisualMaze(maze.MazeId);
            Console.WriteLine(mazeVisual);

            MazePathfinder solver = new MazePathfinder();
            Path ponyPath = solver.Solve(maze);
            Console.WriteLine("Here's the ideal path of the pony, let's see it in action:");
            Console.WriteLine(ponyPath);
            MazeWalker pathPoster = new MazeWalker(ponyAPI, mazeFactory);
            await pathPoster.Walk(ponyPath);

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
