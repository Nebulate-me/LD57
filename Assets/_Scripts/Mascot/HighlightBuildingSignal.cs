namespace _Scripts.Mascot
{
    public readonly struct HighlightBuildingSignal
    {
        public HighlightBuildingSignal(MascotTutorialBuildingTargetType targetType)
        {
            TargetType = targetType;
        }

        public MascotTutorialBuildingTargetType TargetType { get; }
    }

    public readonly struct UnhighlightBuildingSignal
    {
    }
}