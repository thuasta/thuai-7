namespace GameServer.GameLogic;

public interface IBlock
{
    /// <summary>
    /// Whether the block is a wall.
    /// </summary>
    public bool IsWall { get; }

    /// <summary>
    /// Width of the block.
    /// </summary>
    public const float Width = 1;

    /// <summary>
    /// Height of the block.
    /// </summary>
    public const float Height = 1;

    /// <summary>
    /// Items in the block.
    /// </summary>
    public List<IItem> Items { get; }

    /// <summary>
    /// Generate items in the block.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public void GenerateItems(IItem item);

    /// <summary>
    /// Remove items from the block.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public void RemoveItems(IItem item);
}
