using _Scripts.Achievements;

namespace _Scripts.Popups.LevelFinished
{
    public readonly struct AchievementViewUnhoveredSignal
    {
        public Achievement Achievement { get; }
        public AchievementViewUnhoveredSignal(Achievement achievement)
        {
            Achievement = achievement;
        }

        
    }
}