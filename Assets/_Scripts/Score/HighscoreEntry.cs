using System;

namespace _Scripts.Score
{
    [Serializable]
    public class HighscoreEntry
    {
        public string playerName;
        public int score;
        public long timestampUtc;

        public HighscoreEntry(string name, int score)
        {
            playerName = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
            this.score = score;
            timestampUtc = DateTime.UtcNow.Ticks;
        }
    }
}