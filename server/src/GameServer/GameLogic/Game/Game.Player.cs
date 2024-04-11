namespace GameServer.GameLogic;

public partial class Game
{
    public List<Player> AllPlayers { get; private set; } = new();

    public List<Recorder.IRecord> _events = new();

    public void AddPlayer(Player player)
    {
        if (Stage != GameStage.Waiting)
        {
            _logger.Error("Cannot add player: The game is already started.");
            return;
        }

        AllPlayers.Add(player);
        SubscribePlayerEvents(player);
    }

    public void RemovePlayer(Player player)
    {
        AllPlayers.Remove(player);
    }

    public List<Player> GetPlayers()
    {
        return AllPlayers;
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
