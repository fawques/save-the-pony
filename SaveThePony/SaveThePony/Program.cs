using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            Configure(serviceCollection);

            var services = serviceCollection.BuildServiceProvider();
            Console.WriteLine("Creating a client...");
            var ponyAPI = services.GetRequiredService<PonyAPIClient>();

            Console.WriteLine("Sending a request...");
            try
            {
                var response = await ponyAPI.GetMaze(new Guid("407a48c9-74d8-4fc1-a390-e30c93fe3cf1"));
                var data = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response data:");
                Console.WriteLine((object)data);
            }
            catch (HttpRequestException e)
            {
                ILogger l = new Microsoft.Extensions.Logging.Logger<PonyAPIClient>(services.GetRequiredService<ILoggerFactory>());
                l.LogError(e, "error getting maze");
            }

            Console.WriteLine("Press the ANY key to exit...");
            Console.ReadKey();

            services.Dispose();
        }

        public static void Configure(IServiceCollection services)
        {
            services.AddHttpClient("ponyAPI", c =>
            {
                c.BaseAddress = new Uri("https://ponychallenge.trustpilot.com/pony-challenge/maze");
            })
            .AddTypedClient<PonyAPIClient>();
        }

    }
}
