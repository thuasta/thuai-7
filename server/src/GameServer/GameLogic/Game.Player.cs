namespace GameServer.GameLogic;

public partial class Game
{
    public List<Player> AllPlayers { get; private set; } = new();

    public List<Recorder.IRecord> _events = new();

    public void AddPlayer(Player player)
    {
        AllPlayers.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        AllPlayers.Remove(player);
    }

    public List<Player> GetPlayers()
    {
        return AllPlayers;
    }

    public void SubscribePlayerEvents()
    {
        foreach (Player player in AllPlayers)
        {
            player.PlayerAbandonEvent += OnPlayerAbandon;
            player.PlayerAttackEvent += OnPlayerAttack;
            player.PlayerPickUpEvent += OnPlayerPickUp;
            player.PlayerSwitchArmEvent += OnPlayerSwitchArm;
            player.PlayerUseGrenadeEvent += OnPlayerUseGrenade;
            player.PlayerUseMedicineEvent += OnPlayerUseMedicine;
        }
    }

    private void OnPlayerAbandon(object? sender, Player.PlayerAbandonEventArgs e)
    {
        foreach ((IItem.ItemKind itemKind, string itemSpecificName) in e.AbandonedSupplies)
        {
            IItem? item = e.Player.PlayerBackPack.FindItems(itemKind, itemSpecificName);
            if (item != null && item.Count >= e.Number)
            {
                // Remove abandon items from the backpack
                e.Player.PlayerBackPack.RemoveItems(itemKind, itemSpecificName, e.Number);

                // Add abandon items to the ground
                // Get the block at the position of the player
                Position playerPosition = e.Player.PlayerPosition;
                int playerIntX = (int)playerPosition.x;
                int playerIntY = (int)playerPosition.y;
                GameMap.AddSupplies(playerIntX, playerIntY, new Item(itemKind, itemSpecificName, e.Number));
            }

            Recorder.PlayerAbandonRecord record = new()
            {
                Data = new()
                {
                    playerId = e.Player.PlayerId,
                    numb = e.Number,
                    abandonedSupplies = itemSpecificName
                }
            };

            _events.Add(record);
        }
    }

    /// <summary>
    /// Calculate the closest point on the line to the player's position, given that the collision box of the player is a circle.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="playerPosition"></param>
    /// <returns></returns>
    private static (Position closestPoint, bool isSameDirection) CalculatePlayerClosestPoint(Position start, Position end, Position playerPosition)
    {
        // Calculate the direction vector of the line
        Position direction = end - start;

        // Calculate the vector from the start point to the player's position
        Position playerToStart = start - playerPosition;

        // Calculate the dot product of the direction vector and the playerToStart vector
        double dotProduct = Position.Dot(direction, playerToStart);

        // Calculate the closest point on the line to the player's position
        Position closestPoint;
        bool isSameDirection = true;
        if (dotProduct <= 0)
        {
            closestPoint = start;
            isSameDirection = false;
        }
        else if (dotProduct >= direction.LengthSquared())
        {
            closestPoint = end;
        }
        else
        {
            double t = dotProduct / direction.LengthSquared();
            closestPoint = start + direction * t;
        }

        return (closestPoint, isSameDirection);
    }
    private void OnPlayerAttack(object? sender, Player.PlayerAttackEventArgs e)
    {
        // Check if the type weapon is not "Fist"
        if (e.Player.PlayerWeapon is not Fist)
        {
            // Check if the player has enough bullets
            IItem? bullet = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Bullet, "BULLET");
            if (bullet != null && bullet.Count > 0)
            {
                e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Bullet, "BULLET", 1);
                Recorder.PlayerAttackRecord record = new()
                {
                    Data = new()
                    {
                        playerId = e.Player.PlayerId,
                        turgetPosition = new()
                        {
                            x = e.TargetPosition.x,
                            y = e.TargetPosition.y
                        }
                    }
                };

                _events.Add(record);
            }
            else
            {
                throw new InvalidOperationException("Player has no bullet.");
            }
        }

        // Attack the target
        List<Position>? bulletDirections = e.Player.PlayerWeapon.GetBulletDirections(e.Player.PlayerPosition, e.TargetPosition);
        // Traverse all bullets
        if (bulletDirections != null)
        {
            foreach (Position normalizedDirection in bulletDirections)
            {
                foreach (Player targetPlayer in AllPlayers)
                {
                    // Skip the player itself
                    if (targetPlayer == e.Player)
                    {
                        continue;
                    }

                    Position start = targetPlayer.PlayerPosition;
                    Position end = start + normalizedDirection * targetPlayer.PlayerWeapon.Range;
                    // Parameter equation
                    Func<float, Position> getLinearPosition = (float t) =>
                    {
                        return start + normalizedDirection * t;
                    };

                    // The player is represented by a circular collision box, calculating the point closest to the player's collision box along the line (intersection of two lines)
                    (Position closetPoint, bool isSameDirection) = CalculatePlayerClosestPoint(start, end, targetPlayer.PlayerPosition);
                    // Calculate the distance between this point and the player's collision box
                    double distance = Position.Distance(closetPoint, targetPlayer.PlayerPosition);
                    // Not hitting the player
                    if (distance > targetPlayer.PlayerRadius)
                    {
                        continue;
                    }
                    // Different directions
                    if (!isSameDirection)
                    {
                        continue;
                    }

                    // Otherwise, we need to check if hitting obstacles
                    Position endJudgementPoint = closetPoint;
                    // Ray casting, check if hitting the wall, dividing the map into small grids, and calculating all grid intersection points of the ray from start to end
                    bool isHittingWall = false;

                    // If direction.x is close to 0, xGrids will be an empty list, so there's no need for separate handling
                    List<int> xGrids = new();
                    if (start.x < end.x)
                    {
                        for (int i = (int)start.x + 1; i <= (int)end.x; i++)
                        {
                            xGrids.Add(i);
                        }
                    }
                    else
                    {
                        for (int i = (int)end.x - 1; i >= (int)start.x; i--)
                        {
                            xGrids.Add(i);
                        }
                    }
                    foreach (int xGrid in xGrids)
                    {
                        // Calculate all grid intersection points and check if the adjacent point is an obstacle
                        Position intersection = getLinearPosition((float)((xGrid - start.x) / normalizedDirection.x));
                        int intersectionIntX = xGrid;
                        int intersectionIntY = (int)intersection.y;
                        if (GameMap.GetBlock(new Position(intersectionIntX, intersectionIntY))?.IsWall == true)
                        {
                            isHittingWall = true;
                            break;
                        }
                    }

                    // If direction.y is close to 0, yGrids will be an empty list, so there's no need for separate handling
                    List<int> yGrids = new();
                    if (start.y < end.y)
                    {
                        for (int i = (int)start.y + 1; i <= (int)end.y; i++)
                        {
                            yGrids.Add(i);
                        }
                    }
                    else
                    {
                        for (int i = (int)end.y - 1; i >= (int)start.y; i--)
                        {
                            yGrids.Add(i);
                        }
                    }
                    foreach (int yGrid in yGrids)
                    {
                        // Calculate all grid intersection points and check if the adjacent point is an obstacle
                        Position intersection = getLinearPosition((float)((yGrid - start.y) / normalizedDirection.y));
                        int intersectionIntX = (int)intersection.x;
                        int intersectionIntY = yGrid;
                        if (GameMap.GetBlock(new Position(intersectionIntX, intersectionIntY))?.IsWall == true)
                        {
                            isHittingWall = true;
                            break;
                        }
                    }
                    // If isHittingWall is false and cooldown of the weapon is done, the player is hit and health is deducted
                    if (!isHittingWall && e.Player.PlayerWeapon.IsAvailable)
                    {
                        targetPlayer.Health -= e.Player.PlayerWeapon.Damage;
                    }
                }
            }
        }
    }
    private void OnPlayerPickUp(object? sender, Player.PlayerPickUpEventArgs e)
    {
        // Check if the player is close enough to the supply
        if (Position.Distance(e.Player.PlayerPosition, e.TargetPosition) > Constant.PLAYER_PICK_UP_DISTANCE)
        {
            throw new InvalidOperationException("Player is not close enough to the supply.");
        }

        // Check if the supply exists
        IItem? item = (
            GameMap.GetBlock((int)e.TargetPosition.x, (int)e.TargetPosition.y)?
            .Items.Find(i => i.ItemSpecificName == e.TargetSupply && e.Numb < 0 && i.Count <= e.Numb))
            ?? throw new InvalidOperationException("Supply does not exist or the numb is invalid."
        );

        // Add the supply to the player's backpack
        e.Player.PlayerBackPack.AddItems(item.Kind, item.ItemSpecificName, item.Count);

        // Remove the supply from the ground
        GameMap.RemoveSupplies((int)e.TargetPosition.x, (int)e.TargetPosition.y, item);

        Recorder.PlayerPickUpRecord record = new()
        {
            Data = new()
            {
                playerId = e.Player.PlayerId,
                targetPosition = new()
                {
                    x = e.TargetPosition.x,
                    y = e.TargetPosition.y
                },
                targetSupply = item.ItemSpecificName,
                numb = e.Numb
            }
        };

        _events.Add(record);
    }
    private void OnPlayerSwitchArm(object? sender, Player.PlayerSwitchArmEventArgs e)
    {
        //iterate player's backpack to find the weapon with weaponItemId
        //if found, set PlayerWeapon to the weapon and keep its cooldown.
        //if not found, throw new ArgumentException("Weapon not found in backpack.");
        // TODO: 切枪BUG: 可以来回切换两次枪，导致冷却时间不生效
        IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Weapon, e.TargetFirearm);
        if (item != null)
        {
            e.Player.PlayerWeapon = WeaponFactory.CreateFromItem(item);
            Recorder.PlayerSwitchArmRecord record = new()
            {
                Data = new()
                {
                    playerId = e.Player.PlayerId,
                    turgetFirearm = e.TargetFirearm
                }
            };

            _events.Add(record);
        }
        else
        {
            throw new ArgumentException("Weapon not found in backpack.");
        }
    }
    private void OnPlayerUseGrenade(object? sender, Player.PlayerUseGrenadeEventArgs e)
    {
        // Check if the player has grenade
        IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Grenade, "GRENADE");
        if (item != null && item.Count > 0)
        {
            e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Grenade, "GRENADE", 1);
            Recorder.PlayerUseGrenadeRecord record = new()
            {
                Data = new()
                {
                    playerId = e.Player.PlayerId,
                    turgetPosition = new()
                    {
                        x = e.TargetPosition.x,
                        y = e.TargetPosition.y
                    }
                }
            };

            _events.Add(record);
        }
        else
        {
            throw new InvalidOperationException("Player has no grenade.");
        }

        // Generate the grenade
        _allGrenades.Add(new Grenade(e.Player.PlayerPosition, CurrentTick));
    }
    private void OnPlayerUseMedicine(object? sender, Player.PlayerUseMedicineEventArgs e)
    {
        // Check if the player has medicine
        IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Medicine, e.MedicineName);
        if (item != null && item.Count > 0)
        {
            e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Medicine, e.MedicineName, 1);
            e.Player.Health += MedicineFactory.CreateFromItem(item).Heal;

            Recorder.PlayerUseMedicineRecord record = new()
            {
                Data = new()
                {
                    playerId = e.Player.PlayerId,
                    targetMedicine = e.MedicineName
                }
            };

            _events.Add(record);
        }
        else
        {
            throw new InvalidOperationException("Player has no medicine.");
        }
    }
    private void UpdatePlayers()
    {
        foreach (Player player in AllPlayers)
        {
            // Update cooldown of weapons
            player.PlayerWeapon.UpdateCoolDown();
            // Update motion of players
            if (player.PlayerTargetPosition != null)
            {
                Position targetPosition = player.PlayerTargetPosition;
                Position normalizedDirection = (targetPosition - player.PlayerPosition).Normalize();
                Position likelyNextPosition = player.PlayerPosition;

                if (normalizedDirection.LengthSquared() > 0)
                {
                    likelyNextPosition += normalizedDirection * player.Speed;
                }
                if (Position.Distance(player.PlayerPosition, targetPosition) < player.Speed)
                {
                    likelyNextPosition = targetPosition;
                    player.PlayerTargetPosition = null;
                }

                bool isColliding = false;
                double xDistanceFromWall = 1e10;
                double yDistanceFromWall = 1e10;

                int xGrid = (int)player.PlayerPosition.x;
                int yGrid = (int)player.PlayerPosition.y;

                // Iterate through all potential collision positions and calculate distances from walls
                for (int i = (int)(1 + player.Speed); i >= 1; i--)
                {
                    if (normalizedDirection.x != 0 && GameMap.GetBlock(xGrid + Math.Sign(normalizedDirection.x) * i, yGrid)?.IsWall == true)
                    {
                        xDistanceFromWall = normalizedDirection.x > 0 ? Math.Ceiling(normalizedDirection.x) - normalizedDirection.x : normalizedDirection.x - Math.Floor(normalizedDirection.x);
                    }
                }
                for (int i = (int)(1 + player.Speed); i >= 1; i--)
                {
                    if (normalizedDirection.y != 0 && GameMap.GetBlock(xGrid, yGrid + Math.Sign(normalizedDirection.y) * i)?.IsWall == true)
                    {
                        yDistanceFromWall = normalizedDirection.y > 0 ? Math.Ceiling(normalizedDirection.y) - normalizedDirection.y : normalizedDirection.y - Math.Floor(normalizedDirection.y);
                    }
                }

                double xTime = xDistanceFromWall / normalizedDirection.x;
                double yTime = yDistanceFromWall / normalizedDirection.y;

                double xDeltaDistance = xDistanceFromWall - Math.Abs(player.Speed * normalizedDirection.x) - player.PlayerRadius;
                double yDeltaDistance = yDistanceFromWall - Math.Abs(player.Speed * normalizedDirection.y) - player.PlayerRadius;

                // If colliding with wall, the player's position behind can only be the position where it collides with the wall
                if (xTime < yTime)
                {
                    // x direction may collide with wall
                    if (xTime < 1 && xDeltaDistance < 0)
                    {
                        double originalX = player.PlayerPosition.x;
                        player.PlayerPosition.x = xGrid - Math.Sign(normalizedDirection.x) * player.PlayerRadius + (Math.Sign(normalizedDirection.x) + 1) / 2;
                        // Calculate y coordinate based on x coordinate and direction
                        double t = originalX / normalizedDirection.x;
                        player.PlayerPosition.y += t * normalizedDirection.y;
                        isColliding = true;
                    }
                }
                else
                {
                    // y direction may collide with wall
                    if (yTime < 1 && yDeltaDistance < 0)
                    {
                        double originalY = player.PlayerPosition.y;
                        player.PlayerPosition.y = yGrid - Math.Sign(normalizedDirection.y) * player.PlayerRadius + (Math.Sign(normalizedDirection.y) + 1) / 2;
                        // Calculate x coordinate based on y coordinate and direction
                        double t = originalY / normalizedDirection.y;
                        player.PlayerPosition.x += t * normalizedDirection.x;
                        isColliding = true;
                    }
                }

                if (!isColliding)
                {
                    player.PlayerPosition = likelyNextPosition;
                }

                // TODO: Check for front and rear connections? If still not connected, it indicates a calculation error
            }
        }
    }
}
