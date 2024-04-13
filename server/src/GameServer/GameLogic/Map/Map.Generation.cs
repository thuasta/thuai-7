namespace GameServer.GameLogic;

public partial class Map
{
    public void GenerateMap()
    {
        GenerateWalls();
        GenerateSupplies();
    }

    public void GenerateSupplies()
    {
        // Iterate to generate the desired number of supply points
        for (int i = 0; i < _numSupplyPoints; i++)
        {
            // Randomly select position for the supply point
            int x = _random.Next(0, Width);
            int y = _random.Next(0, Height);

            // Check if the selected position is valid (not on an obstacle)
            if (GetBlock(x, y)?.IsWall == false)
            {
                // Randomly determine the number of items for this supply point
                int numItems = _random.Next(_minItemsPerSupply, _maxItemsPerSupply + 1);

                // Generate items and add them to the supply point
                for (int j = 0; j < numItems; j++)
                {
                    // Example: Generate a random item type
                    IItem.ItemKind itemType = (IItem.ItemKind)_random.Next(0, Enum.GetValues(typeof(IItem.ItemKind)).Length);

                    // TODO: According to the item type, randomly sample a specific name for the item
                    string itemSpecificName = WeaponFactory.WeaponNames.ElementAt(_random.Next(0, WeaponFactory.WeaponNames.Length));

                    // Generate a random count for the item (you may customize this part)
                    int itemCount = _random.Next(1, 6); // Random count between 1 and 5

                    // Add the generated item to the supply point
                    AddSupplies(x, y, new Item(itemType, itemSpecificName, itemCount));
                }
            }
        }
    }

    public void GenerateWalls()
    {
        // Clear the map
        Clear();

        // Iterate _tryTimes and place them on the map
        for (int i = 0; i < _tryTimes; i++)
        {
            // Randomly select an obstacle shape
            ObstacleShape shape = _obstacleShapes[_random.Next(0, _obstacleShapes.Count)];

            // Place the obstacle shape on the map
            if (PlaceObstacleShape(shape))
            {
                // Successfully placed the obstacle shape
                // Continue to the next iteration
                continue;
            }
        }
    }

    private bool PlaceObstacleShape(ObstacleShape shape)
    {
        // Randomly select position for the obstacle shape
        int startX = _random.Next(0, Width - shape.MaxWidth);
        int startY = _random.Next(0, Height - shape.MaxHeight);

        // Check if the selected position is valid
        if (IsPositionValid(startX, startY, shape))
        {
            // Place the obstacle shape on the map
            for (int x = 0; x < shape.MaxWidth; x++)
            {
                for (int y = 0; y < shape.MaxHeight; y++)
                {
                    if (shape.IsSolid(x, y))
                    {
                        MapChunk[startX + x, startY + y] = shape.GetBlock(x, y);
                    }
                }
            }

            return true;
        }

        return false;
    }

    private bool IsPositionValid(int startX, int startY, ObstacleShape shape)
    {
        // Check if the position is within the map boundaries
        if (startX < 0 || startY < 0 || startX + shape.MaxWidth >= Width || startY + shape.MaxHeight >= Height)
        {
            return false;
        }

        // Check if the position overlaps with existing obstacles
        for (int x = 0; x < shape.MaxWidth; x++)
        {
            for (int y = 0; y < shape.MaxHeight; y++)
            {
                if (MapChunk[startX + x, startY + y] != null && MapChunk[startX + x, startY + y].IsWall)
                {
                    return false;
                }
            }
        }

        // Check if the position is reachable from the rest of the map
        // Implement connectivity check here (optional)

        return true;
    }


    public void Clear()
    {
        // Clear the map
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                MapChunk[x, y] = new Block(false);
            }
        }
    }

}
