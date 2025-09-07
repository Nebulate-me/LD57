using _Scripts.Popups.GameFinished;
using _Scripts.Score;

namespace _Scripts.Missions
{
    public interface IScoreManager
    {
        int Score { get; }
        bool IsGameFinished { get; }
        GameFinishedReason GameFinishedReason { get; }
        void SubtractScore(int score);
        void StartGame();
        void FinishGame(GameFinishedReason allLevelsCompleted);
        void FinishLevel(int emptyRoomTilesCount);
        ScoreRank GetCurrentRank();
    }
}