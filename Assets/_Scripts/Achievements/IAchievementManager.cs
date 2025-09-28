using System.Collections.Generic;

namespace _Scripts.Achievements
{
    public interface IAchievementManager
    {
        List<Achievement> GetUnlockedAchievements();
        IEnumerable<Achievement> GetUnscoredAchievements();
        int GetUnlockedAchievementsScore();
        void ScoreUnlockedAchievements();
    }
}