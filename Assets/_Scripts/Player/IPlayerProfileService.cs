using System.Collections.Generic;

namespace _Scripts.Player
{
    public interface IPlayerProfileService
    {
        IReadOnlyList<PlayerProfile> Players { get; }
        PlayerProfile CurrentPlayer { get; }
        
        void SetCurrentPlayer(PlayerProfile player);
        bool TrySetCurrentPlayerById(string existingPlayerId, out PlayerProfile currentPlayer);
        bool TrySetCurrentPlayerScore(int score);
        bool TryGetPlayerByName(string playerName, out PlayerProfile player);
        bool TryCreatePlayer(string name, out PlayerProfile player);
    }
}