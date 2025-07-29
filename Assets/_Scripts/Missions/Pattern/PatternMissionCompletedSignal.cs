namespace _Scripts.Missions.Pattern
{
    public struct PatternMissionCompletedSignal
    {
        public PatternMissionDto Dto { get; }
        
        public PatternMissionCompletedSignal(PatternMissionDto patternMissionCardDto)
        {
            Dto = patternMissionCardDto;
        }
    }
}