namespace _Scripts.Missions
{
    public struct MissionCompletedSignal
    {
        public MissionDto Dto { get; }
        
        public MissionCompletedSignal(MissionDto missionCardDto)
        {
            Dto = missionCardDto;
        }
    }
}