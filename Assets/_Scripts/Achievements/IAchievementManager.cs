using System.Collections.Generic;

namespace _Scripts.Achievements
{
    public interface IAchievementManager
    {
        List<Achievement> GetUnlockedAchievements();
        int ScoreUnlockedAchievements();
    }
}