using System.Collections.Generic;

namespace _Scripts.Player
{
    public interface IPlayerProfileService
    {
        IReadOnlyList<PlayerProfile> Players { get; }
        PlayerProfile CurrentPlayer { get; set; }
        bool TrySetCurrentPlayerById(string existingPlayerId, out PlayerProfile currentPlayer);
        bool TryGetPlayerById(string id, out PlayerProfile player);
        bool TryGetPlayerByName(string playerName, out PlayerProfile player);
        bool TryCreatePlayer(string name, out PlayerProfile player);
        bool TrySetPlayerScore(string playerId, int score);
        
    }
}