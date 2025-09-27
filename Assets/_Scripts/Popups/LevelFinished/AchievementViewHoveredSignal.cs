using _Scripts.Achievements;

namespace _Scripts.Popups.LevelFinished
{
    public readonly struct AchievementViewHoveredSignal
    {
        public Achievement Achievement { get; }
        public AchievementViewHoveredSignal(Achievement achievement)
        {
            Achievement = achievement;
        }
    }
}