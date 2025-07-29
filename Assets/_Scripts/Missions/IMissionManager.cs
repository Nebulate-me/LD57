using _Scripts.Missions.Apartment;

namespace _Scripts.Missions
{
    public interface IMissionManager
    {
        int CompletableMissionsCount { get; }
        void CompleteMission(PatternMissionCardView patternMissionCard);
        void CompleteMission(ApartmentMissionCardView apartmentMissionCard);
    }
}