using GameServer.Geometry;
using GameServer.Geometry.Shapes;

namespace GameServer.GameLogic;

public partial class Game
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

        if (e.Player.LastPickUpOrAbandonTick is not null && CurrentTick - e.Player.LastPickUpOrAbandonTick < Constant.PLAYER_PICK_UP_OR_ABANDON_COOLDOWN)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot abandon or pick up supplies too frequently.");
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
            lock (_lock)
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
                                $"[Player {e.Player.PlayerId}] Cannot abandon more than one {itemSpecificName} at once."
                            );
                            return;
                        }

                        e.Player.PlayerArmor = Armor.DefaultArmor;
                        break;

                    case IItem.ItemKind.Weapon:
                        if (e.Number > 1)
                        {
                            _logger.Error(
                                $"[Player {e.Player.PlayerId}] Cannot abandon more than one {itemSpecificName} at once."
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
                        token = e.Player.Token,
                        numb = e.Number,
                        abandonedSupplies = itemSpecificName
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to abandon supplies: {ex.Message}");
            _logger.Debug($"{ex}");
        }

        e.Player.LastPickUpOrAbandonTick = CurrentTick;
    }

    private void OnPlayerAttack(object? sender, Player.PlayerAttackEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot attack when the game is at stage {Stage}.");
            return;
        }
        if (e.Player.LastSwitchArmTick is not null
            && CurrentTick - e.Player.LastSwitchArmTick < Constant.PLAYER_FIREARM_PREPARATION_TICK)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot attack instantly after switching arm.");
            return;
        }

        try
        {
            lock (_lock)
            {
                if (!e.Player.PlayerWeapon.IsAvailable)
                {
                    _logger.Error($"[Player {e.Player.PlayerId}] Weapon is still in cool down.");
                    return;
                }

                // Check if the type weapon requires bullets
                if (e.Player.PlayerWeapon.RequiredBulletNum > 0)
                {
                    // Check if the player has enough bullets
                    IItem? bullet = e.Player.PlayerBackPack.FindItems(IItem.ItemKind.Bullet, Constant.Names.BULLET);
                    if (bullet is null || bullet.Count < e.Player.PlayerWeapon.RequiredBulletNum)
                    {
                        _logger.Error($"[Player {e.Player.PlayerId}] No enough bullet.");
                        return;
                    }

                    e.Player.PlayerBackPack.RemoveItems(
                        IItem.ItemKind.Bullet,
                        Constant.Names.BULLET,
                        e.Player.PlayerWeapon.RequiredBulletNum
                    );
                }

                // Attack the target
                List<Position>? bulletDirections
                    = e.Player.PlayerWeapon.GetBulletDirections(e.Player.PlayerPosition, e.TargetPosition);
                double realRange = e.Player.PlayerWeapon.Range;

                // Traverse all bullets
                if (bulletDirections != null)
                {
                    foreach (Position normalizedDirection in bulletDirections)
                    {
                        Position start = e.Player.PlayerPosition;
                        Position end = start + normalizedDirection * e.Player.PlayerWeapon.Range;
                        Position realEnd = GameMap.GetRealEndPositon(start, end);
                        realRange = Math.Min(realRange, Position.Distance(start, realEnd));

                        foreach (Player targetPlayer in AllPlayers)
                        {
                            // Skip the player itself
                            if (targetPlayer.PlayerId == e.Player.PlayerId)
                            {
                                continue;
                            }

                            if (CollisionDetector.IsCrossing(
                                new Segment(start, realEnd)
                                , new Circle(targetPlayer.PlayerPosition, Constant.PLAYER_COLLISION_BOX)
                            ) == true)
                            {
                                targetPlayer.TakeDamage(e.Player.PlayerWeapon.Damage);
                            }
                        }
                    }
                }

                Recorder.PlayerAttackRecord record = new()
                {
                    Data = new()
                    {
                        playerId = e.Player.PlayerId,
                        token = e.Player.Token,
                        turgetPosition = new()
                        {
                            x = e.TargetPosition.x,
                            y = e.TargetPosition.y
                        },
                        range = realRange,
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to attack: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private void OnPlayerPickUp(object? sender, Player.PlayerPickUpEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up supplies when the game is at stage {Stage}.");
            return;
        }
        if (e.Numb <= 0)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up supplies with non-positive number.");
            return;
        }
        if (e.TargetSupply == Constant.Names.FIST || e.TargetSupply == Constant.Names.NO_ARMOR)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up {e.TargetSupply}.");
            return;
        }
        if (e.Player.LastPickUpOrAbandonTick is not null && CurrentTick - e.Player.LastPickUpOrAbandonTick < Constant.PLAYER_PICK_UP_OR_ABANDON_COOLDOWN)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up or abandon supplies too frequently.");
            return;
        }

        try
        {
            lock (_lock)
            {
                switch (IItem.GetItemKind(e.TargetSupply))
                {
                    case IItem.ItemKind.Armor:
                        if (e.Numb > 1)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up more than one armor at once.");
                            return;
                        }

                        IItem? armorItem = GameMap.GetBlock(e.Player.PlayerPosition)?.Items.Find(
                                        i => i.ItemSpecificName == e.TargetSupply
                                    );

                        if (armorItem is null || armorItem.Count < e.Numb)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] No enough {e.TargetSupply}.");
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
                        GameMap.RemoveSupplies((int)e.Player.PlayerPosition.x, (int)e.Player.PlayerPosition.y, armorItem);
                        break;

                    case IItem.ItemKind.Weapon:
                        if (e.Player.IsWeaponSlotFull == true)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] Weapon slot is already full.");
                            return;
                        }
                        if (e.Numb > 1)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] Cannot pick up more than one weapon at once.");
                            return;
                        }

                        IItem? weaponItem = GameMap.GetBlock(e.Player.PlayerPosition)?.Items.Find(
                                        i => i.ItemSpecificName == e.TargetSupply
                                    );
                        if (weaponItem is null || weaponItem.Count < e.Numb)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] No enough {e.TargetSupply}.");
                            return;
                        }
                        if (e.Player.WeaponSlot.Any(w => w.Name == e.TargetSupply) == true)
                        {
                            _logger.Error(
                                $"[Player {e.Player.PlayerId}] Cannot own more than one {e.TargetSupply}."
                            );
                            return;
                        }

                        e.Player.WeaponSlot.Add(WeaponFactory.CreateFromItem(weaponItem));
                        GameMap.RemoveSupplies((int)e.Player.PlayerPosition.x, (int)e.Player.PlayerPosition.y, weaponItem);

                        break;

                    default:
                        // Check if the supply exists
                        IItem? generalItem = GameMap.GetBlock(e.Player.PlayerPosition)?.Items.Find(
                                        i => i.ItemSpecificName == e.TargetSupply
                                    );

                        if (generalItem is null || generalItem.Count < e.Numb)
                        {
                            _logger.Error($"[Player {e.Player.PlayerId}] No enough {e.TargetSupply}.");
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
                            _logger.Debug($"{ex}");
                            return;
                        }

                        // Remove the supply from the ground
                        GameMap.RemoveSupplies(
                            (int)e.Player.PlayerPosition.x,
                            (int)e.Player.PlayerPosition.y,
                            new Item(generalItem.Kind, generalItem.ItemSpecificName, e.Numb)
                        );

                        break;
                }

                Recorder.PlayerPickUpRecord record = new()
                {
                    Data = new()
                    {
                        playerId = e.Player.PlayerId,
                        token = e.Player.Token,
                        targetSupply = e.TargetSupply,
                        numb = e.Numb
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to pick up supplies: {ex.Message}");
            _logger.Debug($"{ex}");
        }

        e.Player.LastPickUpOrAbandonTick = CurrentTick;
    }

    private void OnPlayerSwitchArm(object? sender, Player.PlayerSwitchArmEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot switch arm when the game is at stage {Stage}.");
            return;
        }

        try
        {
            lock (_lock)
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
                        e.Player.LastSwitchArmTick = CurrentTick;
                        break;
                    }
                }

                Recorder.PlayerSwitchArmRecord record = new()
                {
                    Data = new()
                    {
                        playerId = e.Player.PlayerId,
                        token = e.Player.Token,
                        turgetFirearm = e.TargetFirearm
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to switch arm: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private void OnPlayerUseGrenade(object? sender, Player.PlayerUseGrenadeEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot use grenade when the game is at stage {Stage}.");
            return;
        }

        if (e.Player.LastUseGrenadeTick is not null && CurrentTick - e.Player.LastUseGrenadeTick < Constant.PLAYER_USE_GRENADE_COOLDOWN)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot use grenades too frequently.");
            return;
        }

        try
        {
            lock (_lock)
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
                        token = e.Player.Token,
                        turgetPosition = new()
                        {
                            x = e.TargetPosition.x,
                            y = e.TargetPosition.y
                        }
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to use grenade: {ex.Message}");
            _logger.Debug($"{ex}");
        }

        e.Player.LastUseGrenadeTick = CurrentTick;
    }

    private void OnPlayerUseMedicine(object? sender, Player.PlayerUseMedicineEventArgs e)
    {
        if (Stage != GameStage.Fighting)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Cannot use medicine when the game is at stage {Stage}.");
            return;
        }

        try
        {
            lock (_lock)
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
                        token = e.Player.Token,
                        targetMedicine = e.MedicineName
                    }
                };

                _events.Add(record);
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to use medicine: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private void OnPlayerTeleport(object? sender, Player.PlayerTeleportEventArgs e)
    {
        if (Stage != GameStage.Preparing)
        {
            _logger.Error(
                $"[Player {e.Player.PlayerId}] Teleportation is only allowed at stage Preparing (actual stage {Stage})."
            );
            return;
        }

        try
        {
            lock (_lock)
            {
                if (GameMap.GetBlock(e.TargetPosition) is not null && GameMap.GetBlock(e.TargetPosition)?.IsWall == false)
                {
                    e.Player.PlayerPosition = e.TargetPosition;
                }
                else
                {
                    _logger.Error($"[Player {e.Player.PlayerId}] Cannot teleport to a wall or outside of the map.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"[Player {e.Player.PlayerId}] Failed to teleport: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }
}
