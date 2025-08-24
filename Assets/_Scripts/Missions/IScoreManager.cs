namespace _Scripts.Missions
{
    public interface IScoreManager
    {
        int Score { get; }
        void StartGame(string playerName);
        void FinishLevel(int emptyRoomTilesCount);
    }
}