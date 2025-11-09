namespace _Scripts.Mascot
{
    public readonly struct HighlightBuildingSignal
    {
        public HighlightBuildingSignal(BuildingHighlightType buildingHighlightType)
        {
            BuildingHighlightType = buildingHighlightType;
        }

        public BuildingHighlightType BuildingHighlightType { get; }
    }

    public enum BuildingHighlightType
    {
        Floor = 0,
        AllWindows = 1
    }

    public readonly struct UnhighlightBuildingSignal
    {
    }
}