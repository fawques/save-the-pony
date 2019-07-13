using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaveThePony.Interfaces;
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
            ServiceCollection serviceCollection = ConfigureServices();

            var services = serviceCollection.BuildServiceProvider();
            IPonyAPIClient ponyAPI = services.GetRequiredService<IPonyAPIClient>();
            IMazeFactory factory = services.GetRequiredService<IMazeFactory>();
            IMazePathfinder pathfinder = services.GetRequiredService<IMazePathfinder>();
            IMazeWalker walker = services.GetRequiredService<IMazeWalker>();

            Maze maze = null;
            while (maze is null)
            {
                Console.WriteLine("Enter a maze GUID or press enter for a new maze:");
                string input = Console.ReadLine();
                if (input != "")
                {
                    if (Guid.TryParse(input, out Guid mazeId))
                    {
                        maze = await factory.FromID(mazeId);
                    }
                    else
                    {
                        Console.WriteLine("Could not parse mazeId");
                    }
                }
                else
                {
                    maze = await factory.Create();
                    Console.WriteLine($"Maze ID {maze.MazeId} created.");

                }
            }
            Console.WriteLine(await ponyAPI.GetVisualMaze(maze.MazeId));

            Path ponyPath = pathfinder.Solve(maze);
            Console.WriteLine("Here's the ideal path of the pony, let's see it in action:");
            Console.WriteLine(ponyPath);
            await walker.Walk(ponyPath);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            services.Dispose();
        }

        static ServiceCollection ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IPonyAPIClient, PonyAPIClient>();
            serviceCollection.AddScoped<IMazeFactory, MazeFactory>();
            serviceCollection.AddScoped<IMazePathfinder, MazePathfinder>();
            serviceCollection.AddScoped<IMazeWalker, MazeWalker>();

            serviceCollection.AddHttpClient<IPonyAPIClient, PonyAPIClient>();
            return serviceCollection;
        }
    }
}
