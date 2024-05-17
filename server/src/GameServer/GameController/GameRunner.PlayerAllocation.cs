using System.Collections.Concurrent;

using GameServer.GameLogic;
using GameServer.Geometry;

namespace GameServer.GameController;

public partial class GameRunner
{

    private readonly ConcurrentDictionary<string, int> _tokenToPlayerId = new();
    private int _nextPlayerId = 0;

    private readonly object _allocationLock = new();

    public void AllocatePlayer(string token)
    {
        string playerTokenForLogging
            = token.Length > 16 ? string.Concat(token.AsSpan(0, 16), "...") : token;

        if ((WhiteListMode == true) && (WhiteList.Any(t => t == token) == false))
        {
            _logger.Error($"Token {playerTokenForLogging} is not in the whitelist.");
            return;
        }
        if (_tokenToPlayerId.ContainsKey(token))
        {
            _logger.Debug($"Token {playerTokenForLogging} is already in the game.");
            return;
        }

        _logger.Information(
            $"Adding player {_nextPlayerId} with token \"{playerTokenForLogging}\" to the game."
        );

        try
        {
            lock (_allocationLock)
            {
                if (Game.AddPlayer(
                    new Player(
                        token,
                        _nextPlayerId,
                        Constant.PLAYER_MAXIMUM_HEALTH,
                        Constant.PLAYER_SPEED_PER_TICK,
                        new Position(0, 0)
                    )
                ) == false)
                {
                    _logger.Error(
                        $"Failed to add player with token \"{playerTokenForLogging}\" to the game."
                    );
                    return;
                }

                _tokenToPlayerId[token] = _nextPlayerId;
                _nextPlayerId++;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(
                $"Failed to add player with token \"{playerTokenForLogging}\" to the game: {ex.Message}"
            );
            _logger.Debug($"{ex}");
        }
    }

    public void AllocatePlayer(IEnumerable<string> tokens)
    {
        foreach (string token in tokens)
        {
            AllocatePlayer(token);
        }
    }
}
