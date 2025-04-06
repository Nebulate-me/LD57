namespace _Scripts.Missions
{
    public interface IMissionManager
    {
        int CompletableMissionsCount { get; }
        void CompleteMission(MissionCardView missionCard);
    }
}