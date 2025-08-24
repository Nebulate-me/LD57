namespace _Scripts.Game
{
    public readonly struct LevelSetupCompletedSignal
    {
        public Level Level { get; }
        
        public LevelSetupCompletedSignal(Level level)
        {
            Level = level;
        }
    }
}