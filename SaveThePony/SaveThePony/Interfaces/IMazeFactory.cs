using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony.Interfaces
{
    public interface IMazeFactory
    {
        Task<Maze> Create();
        Task<Maze> Create(int width, int height, string ponyName, int difficulty);
        Task<Maze> FromID(Guid mapId);
        Maze FromJson(string jsonMaze);
    }
}
