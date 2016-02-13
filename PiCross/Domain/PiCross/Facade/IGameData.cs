using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Facade
{
    public interface IGameData
    {
        IPuzzleLibrary PuzzleLibrary { get; }

        IPlayerDatabase PlayerDatabase { get; }
    }
}
