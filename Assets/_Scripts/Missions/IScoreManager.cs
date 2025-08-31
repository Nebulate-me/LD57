using _Scripts.Score;

namespace _Scripts.Missions
{
    public interface IScoreManager
    {
        int Score { get; }
        int CompletedMissionsCount { get; }
        string PlayerName { get; set;  }
        void StartGame();
        void FinishLevel(int emptyRoomTilesCount);
        ScoreRank GetCurrentRank();
    }
}