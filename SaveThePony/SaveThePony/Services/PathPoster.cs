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
            foreach (var step in path.Steps)
            {
                await ponyClient.PostStep(new Guid(), "test");
            }
        }
    }
}
