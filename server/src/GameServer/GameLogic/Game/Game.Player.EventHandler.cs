using System.Reflection.Metadata;

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
            _logger.Error(
                $"[Player {e.Player.PlayerId}] Cannot abandon supplies when the game is at stage {Stage}."
            );
            return;
        }
        if (e.Number <= 0)
        {
            _logger.Error(
                $"[Player {e.Player.PlayerId}] Numbers of abandoned supplies should be positive (actual {e.Number})."
            );
            return;
        }

        Position playerPosition = e.Player.PlayerPosition;
        int playerIntX = (int)playerPosition.x;
        int playerIntY = (int)playerPosition.y;

        if (GameMap.GetBlock(playerPosition) is null || GameMap.GetBlock(playerPosition)?.IsWall == true)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot abandon supplies at an invalid position.");
            return;
        }

        try
        {
            IItem.ItemKind itemKind = e.AbandonedSupplies.ItemKind;
            string itemSpecificName = e.AbandonedSupplies.ItemSpecificName;
            if (itemSpecificName == Constant.Names.FIST || itemSpecificName == Constant.Names.NO_ARMOR)
            {
                _logger.Error($"[Player {e.Player.PlayerId}] Cannot abandon {itemSpecificName}.");
                return;
            }

            switch (itemKind)
            {
                case IItem.ItemKind.Armor:
                    if (itemSpecificName != e.Player.PlayerArmor.ItemSpecificName)
                    {
                        _logger.Error(
                            $"[Player {e.Player.PlayerId}] Cannot abandon {itemSpecificName}: Not wearing it."
                        );
                        return;
                    }
                    if (e.Number > 1)
                    {
                        _logger.Error(
                            $"[Player {e.Player.PlayerId}] Cannot abandon more than one {itemSpecificName}."
                        );
                        return;
                    }

                    GameMap.AddSupplies(playerIntX, playerIntY, ArmorFactory.ToItem(e.Player.PlayerArmor, 1));
                    e.Player.PlayerArmor = Armor.DefaultArmor;
                    break;

                case IItem.ItemKind.Weapon:
                    if (e.Number > 1)
                    {
                        _logger.Error(
                            $"[Player {e.Player.PlayerId}] Cannot abandon more than one {itemSpecificName}."
                        );
                        return;
                    }
                    if (e.Player.WeaponSlot.Any(w => w.Name == itemSpecificName) == false)
                    {
                        _logger.Error(
                            $"[Player {e.Player.PlayerId}] Doesn't have {itemSpecificName}."
                        );
                        return;
                    }

                    for (int i = 0; i < e.Player.WeaponSlot.Count; i++)
                    {
                        if (e.Player.WeaponSlot[i].Name == itemSpecificName)
                        {
                            GameMap.AddSupplies(playerIntX, playerIntY, WeaponFactory.ToItem(e.Player.WeaponSlot[i]));
                            if (e.Player.PlayerWeapon.Name == itemSpecificName)
                            {
                                e.Player.PlayerWeapon = IWeapon.DefaultWeapon;
                            }
                            e.Player.WeaponSlot.RemoveAt(i);
                            break;
                        }
                    }

                    break;

                default:
                    IItem? item = e.Player.PlayerBackPack.FindItems(itemKind, itemSpecificName);
                    if (item is null || item.Count < e.Number)
                    {
                        _logger.Error(
                            $"[Player {e.Player.PlayerId}] Cannot abandon {e.Number} {itemSpecificName}(s)."
                        );
                        return;
                    }

                    // Remove abandon items from the backpack
                    e.Player.PlayerBackPack.RemoveItems(itemKind, itemSpecificName, e.Number);

                    // Add abandon items to the ground
                    // Get the block at the position of the player
                    GameMap.AddSupplies(playerIntX, playerIntY, new Item(itemKind, itemSpecificName, e.Number));

                    break;
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
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to abandon supplies: {ex.Message}");
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
            _logger.Error($"Player {e.Player.PlayerId} cannot attack when the game is at stage {Stage}.");
            return;
        }

        try
        {
            if (!e.Player.PlayerWeapon.IsAvailable)
            {
                _logger.Error($"[Player {e.Player.PlayerId}] Weapon is not available.");
                return;
            }

            // Check if the type weapon requires bullets
            if (e.Player.PlayerWeapon.RequiresBullet == true)
            {
                // Check if the player has enough bullets
                IItem? bullet = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Bullet, Constant.Names.BULLET);
                if (bullet is null || bullet.Count <= 0)
                {
                    _logger.Error($"[Player {e.Player.PlayerId}] No bullet.");
                    return;
                }

                e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Bullet, Constant.Names.BULLET, 1);
            }

            // Attack the target
            List<Position>? bulletDirections
                = e.Player.PlayerWeapon.GetBulletDirections(e.Player.PlayerPosition, e.TargetPosition);
            // Traverse all bullets
            if (bulletDirections != null)
            {
                foreach (Position normalizedDirection in bulletDirections)
                {
                    foreach (Player targetPlayer in AllPlayers)
                    {
                        // Skip the player itself
                        if (targetPlayer.PlayerId == e.Player.PlayerId)
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
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to attack: {ex.Message}");
        }
    }

    private void OnPlayerPickUp(object? sender, Player.PlayerPickUpEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot pick up supplies when the game is at stage {Stage}.");
            return;
        }
        if (e.Numb <= 0)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up supplies with non-positive number.");
            return;
        }
        if (Position.Distance(e.Player.PlayerPosition, e.TargetPosition) > Constant.PLAYER_PICK_UP_DISTANCE)
        {
            _logger.Error($"Player {e.Player.PlayerId} is not close enough to the supply.");
            return;
        }
        if (GameMap.GetBlock(e.TargetPosition) is null || GameMap.GetBlock(e.TargetPosition)?.IsWall == true)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up supplies at an invalid position.");
            return;
        }
        if (e.TargetSupply == Constant.Names.FIST || e.TargetSupply == Constant.Names.NO_ARMOR)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up {e.TargetSupply}.");
            return;
        }

        try
        {
            switch (IItem.GetItemKind(e.TargetSupply))
            {
                case IItem.ItemKind.Armor:
                    if (e.Numb > 1)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up more than one armor.");
                        return;
                    }

                    IItem? armorItem = GameMap.GetBlock(e.TargetPosition)?.Items.Find(
                                    i => i.ItemSpecificName == e.TargetSupply
                                );

                    if (armorItem is null || armorItem.Count < e.Numb)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Supply not found or no enough supplies.");
                        return;
                    }

                    if (e.Player.PlayerArmor.ItemSpecificName != Constant.Names.NO_ARMOR)
                    {
                        // Abandon current armor to wear new armor
                        e.Player.PlayerAbandon(
                            1, (IItem.ItemKind.Armor, e.Player.PlayerArmor.ItemSpecificName)
                        );
                    }
                    e.Player.PlayerArmor = ArmorFactory.CreateFromItem(armorItem);
                    GameMap.RemoveSupplies((int)e.TargetPosition.x, (int)e.TargetPosition.y, armorItem);
                    break;

                case IItem.ItemKind.Weapon:
                    if (e.Player.WeaponSlot.Count >= Constant.PLAYER_WEAPON_SLOT_SIZE)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Weapon slot is already full.");
                        return;
                    }
                    if (e.Numb > 1)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up more than one weapon.");
                        return;
                    }

                    IItem? weaponItem = GameMap.GetBlock(e.TargetPosition)?.Items.Find(
                                    i => i.ItemSpecificName == e.TargetSupply
                                );
                    if (weaponItem is null || weaponItem.Count < e.Numb)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Supply not found or no enough supplies.");
                        return;
                    }
                    if (e.Player.WeaponSlot.Any(w => w.Name == e.TargetSupply) == true)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Cannot own more than one {e.TargetSupply}.");
                        return;
                    }

                    e.Player.WeaponSlot.Add(WeaponFactory.CreateFromItem(weaponItem));
                    GameMap.RemoveSupplies((int)e.TargetPosition.x, (int)e.TargetPosition.y, weaponItem);

                    break;

                default:
                    // Check if the supply exists
                    IItem? generalItem = GameMap.GetBlock(e.TargetPosition)?.Items.Find(
                                    i => i.ItemSpecificName == e.TargetSupply
                                );

                    if (generalItem is null || generalItem.Count < e.Numb)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Supply not found or no enough supplies.");
                        return;
                    }

                    // Add the supply to the player's backpack
                    try
                    {
                        e.Player.PlayerBackPack.AddItems(
                            generalItem.Kind, generalItem.ItemSpecificName, e.Numb
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] Failed to pick up supplies: {ex.Message}");
                        return;
                    }

                    // Remove the supply from the ground
                    GameMap.RemoveSupplies(
                        (int)e.TargetPosition.x,
                        (int)e.TargetPosition.y,
                        new Item(generalItem.Kind, generalItem.ItemSpecificName, e.Numb)
                    );

                    break;
            }

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
                    targetSupply = e.TargetSupply,
                    numb = e.Numb
                }
            };

            _events.Add(record);
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to pick up supplies: {ex.Message}");
        }
    }

    private void OnPlayerSwitchArm(object? sender, Player.PlayerSwitchArmEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot switch arm when the game is at stage {Stage}.");
            return;
        }

        try
        {
            if (e.TargetFirearm == Constant.Names.FIST)
            {
                e.Player.PlayerWeapon = IWeapon.DefaultWeapon;
            }
            else
            {
                if (e.Player.WeaponSlot.Any(w => w.Name == e.TargetFirearm) == false)
                {
                    _logger.Error($"[Player {e.Player.PlayerId}] Doesn't have {e.TargetFirearm}.");
                    return;
                }

                for (int i = 0; i < e.Player.WeaponSlot.Count; i++)
                {
                    if (e.Player.WeaponSlot[i].Name == e.TargetFirearm)
                    {
                        e.Player.PlayerWeapon = e.Player.WeaponSlot[i];
                        break;
                    }
                }
            }

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
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to switch arm: {ex.Message}");
        }
    }

    private void OnPlayerUseGrenade(object? sender, Player.PlayerUseGrenadeEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot use grenade when the game is at stage {Stage}.");
            return;
        }

        try
        {
            // Check if the player has grenade
            IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Grenade, Constant.Names.GRENADE);
            if (item is null || item.Count <= 0)
            {
                _logger.Error($"[Player {e.Player.PlayerId}] Failed to use grenade: Player has no grenade.");
            }
            if (GameMap.GetBlock(e.TargetPosition) is null || GameMap.GetBlock(e.TargetPosition)?.IsWall == true)
            {
                _logger.Error($"[Player {e.Player.PlayerId}] Cannot throw grenade to a wall or outside of the map.");
            }
            if (Position.Distance(e.TargetPosition, e.Player.PlayerPosition) > Constant.GRENADE_THROW_RADIUS)
            {
                _logger.Error(
                    $"[Player {e.Player.PlayerId}] Cannot throw grenade farther than {Constant.GRENADE_THROW_RADIUS}."
                );
            }

            e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Grenade, Constant.Names.GRENADE, 1);
            // Generate the grenade
            _allGrenades.Add(new Grenade(e.TargetPosition, CurrentTick));

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
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to use grenade: {ex.Message}");
        }
    }

    private void OnPlayerUseMedicine(object? sender, Player.PlayerUseMedicineEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"Player {e.Player.PlayerId} cannot use medicine when the game is at stage {Stage}.");
            return;
        }

        try
        {
            // Check if the player has medicine
            IItem? item = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Medicine, e.MedicineName);
            if (item == null || item.Count <= 0)
            {
                _logger.Error($"[Player {e.Player.PlayerId}] Failed to use medicine: Player has no medicine.");
                return;
            }

            e.Player.PlayerBackPack.RemoveItems(IItem.ItemKind.Medicine, e.MedicineName, 1);
            e.Player.TakeHeal(MedicineFactory.CreateFromItem(item).Heal);

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
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to use medicine: {ex.Message}");
        }
    }

    private void OnPlayerTeleport(object? sender, Player.PlayerTeleportEventArgs e)
    {
        if (Stage != GameStage.Preparing)
        {
            _logger.Error($"Teleportation is only allowed at stage Preparing (actual stage {Stage}).");
        }

        try
        {
            if (GameMap.GetBlock(e.TargetPosition) is not null && GameMap.GetBlock(e.TargetPosition)?.IsWall == false)
            {
                e.Player.PlayerPosition = e.TargetPosition;
            }
            else
            {
                _logger.Error($"Player {e.Player.PlayerId} cannot teleport to a wall or outside of the map.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to teleport: {ex.Message}");
        }
    }
}
