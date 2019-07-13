using SaveThePony.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SaveThePony.Interfaces
{
    public interface IMazeWalker
    {
        Task Walk(Path path);
    }
}
