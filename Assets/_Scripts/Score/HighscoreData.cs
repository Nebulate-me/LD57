using System;
using System.Collections.Generic;

namespace _Scripts.Score
{
    [Serializable]
    public class HighscoreData
    {
        public List<HighscoreEntry> entries = new();
    }
}