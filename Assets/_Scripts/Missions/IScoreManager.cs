using _Scripts.Score;

namespace _Scripts.Missions
{
    public interface IScoreManager
    {
        int Score { get; }
        void SubtractScore(int score);
        void StartGame();
        void FinishLevel(int emptyRoomTilesCount);
        ScoreRank GetCurrentRank();
        
    }
}