using System.Numerics;

namespace GameServer.GameLogic;

public interface IBarrier
{
    /// <summary>
    /// generate the chunk Id of Barrier
    /// </summary>
    public void GenerateBarrier();
}
