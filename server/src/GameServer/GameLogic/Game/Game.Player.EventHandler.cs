namespace GameServer.GameLogic;

public partial class Game : IGame
{
    public void SubscribePlayerEvents(Player player)
    {
        player.PlayerAbandonEvent += OnPlayerAbandon;
        player.PlayerAttackEvent += OnPlayerAttack;
        player.PlayerPickUpEvent += OnPlayerPickUp;
        player.PlayerSwitchArmEvent += OnPlayerSwitchArm;
        player.PlayerUseGrenadeEvent += OnPlayerUseGrenade;
        player.PlayerUseMedicineEvent += OnPlayerUseMedicine;
        player.PlayerTeleportEvent += OnPlayerTeleport;
    }

    private void OnPlayerAbandon(object? sender, Player.PlayerAbandonEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot abandon supplies when the game is not at stage Fighting.");
            return;
        }

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
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot attack when the game is not at stage Fighting.");
            return;
        }

        // Check if the type weapon is not "Fist"
        if (e.Player.PlayerWeapon is not Fist)
        {
            // Check if the player has enough bullets
            IItem? bullet = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Bullet, "BULLET");
            if (bullet != null && bullet.Count > 0)
            {
                e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Bullet, "BULLET", 1);
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

    private void OnPlayerPickUp(object? sender, Player.PlayerPickUpEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot pick up supplies when the game is not at stage Fighting.");
            return;
        }

        // Check if the player is close enough to the supply
        if (Position.Distance(e.Player.PlayerPosition, e.TargetPosition) > Constant.PLAYER_PICK_UP_DISTANCE)
        {
            _logger.Error($"Player {e.Player.PlayerId} is not close enough to the supply.");
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
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot switch arm when the game is not at stage Fighting.");
            return;
        }

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
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot use grenade when the game is not at stage Fighting.");
            return;
        }
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
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot use medicine when the game is not at stage Fighting.");
            return;
        }

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

    private void OnPlayerTeleport(object? sender, Player.PlayerTeleportEventArgs e)
    {
        if (Stage != GameStage.Preparing)
        {
            _logger.Error("Teleportation is only allowed at stage Preparing.");
        }

        if (GameMap.GetBlock(e.TargetPosition.x, e.TargetPosition.y)?.IsWall == false)
        {
            e.Player.PlayerPosition = e.TargetPosition;
        }
        else
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot teleport to a wall or outside of the map.");
        }
    }
}
