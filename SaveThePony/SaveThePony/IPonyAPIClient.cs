using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaveThePony
{
    public interface IPonyAPIClient
    {
        Task<HttpResponseMessage> GetMaze(Guid mazeId);
    }
}