namespace GameServer.GameLogic;

public interface IMap
{
    /// <summary>
    /// Get the block by the position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public IBlock? GetBlock(int x, int y);
    public IBlock? GetBlock(float x, float y);
    public IBlock? GetBlock(Position position);

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
    /// Add the supplies in map
    /// </summary>
    public void AddSupplies(int x, int y, IItem item);

    /// <summary>
    /// Remove the supplies in map
    /// </summary>
    public void RemoveSupplies(int x, int y, IItem item);
}
