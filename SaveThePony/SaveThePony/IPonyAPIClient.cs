using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaveThePony
{
    public interface IPonyAPIClient
    {
        Task<HttpResponseMessage> CreateMaze(int width, int height, string ponyName, int difficulty);
        Task<HttpResponseMessage> GetMaze(Guid mazeId);
    }
}