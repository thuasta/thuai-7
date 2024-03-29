namespace GameServer.GameLogic;

public interface IMap
{
    /// <summary>
    /// Chunks in the map
    /// </summary>
    public List<List<IBlock>> MapChunk { get; }

    /// <summary>
    /// Generate the barrier and supplies
    /// </summary>
    public void GenerateMap();

    /// <summary>
    /// Generate the supplies and their position
    /// </summary>
    public void GenerateSupplies();

    /// <summary>
    /// Generate the walls in the map
    /// </summary>
    public void GenerateWalls();

    /// <summary>
    /// Update the supplies in map
    /// </summary>
    public void UpdateSupplies();

}
