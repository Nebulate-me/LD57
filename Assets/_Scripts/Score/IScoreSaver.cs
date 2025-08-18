using System.Collections.Generic;

namespace _Scripts.Score
{
    public interface IScoreSaver
    {
        void SaveToTextFile();
        void SubmitScore(int score, string playerName = null);
        void ClearAll();
        List<HighscoreEntry> Entries { get; }
    }
}