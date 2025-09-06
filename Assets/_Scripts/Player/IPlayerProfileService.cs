namespace _Scripts.Player
{
    public interface IPlayerProfileService
    {
        bool TryGetPlayer(string id, out PlayerProfile player);
        bool TryCreatePlayer(string id, string name, out PlayerProfile player);
        bool TrySetPlayerScore(string playerId, int score);
    }
}