namespace _Scripts.Missions
{
    public interface IScoreManager
    {
        int Score { get; }
        string RankName { get; }
        string RankDescription { get; }
        string PlayerName { get; }
        void StartGame(string playerName);
        void FinishLevel(int emptyRoomTilesCount);
    }
}