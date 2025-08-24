using _Scripts.Missions.Apartment;

namespace _Scripts.Missions
{
    public interface IMissionManager
    {
        int CompletableMissionsCount { get; }
        void CompleteMission(ApartmentMissionCardView apartmentMissionCard);
        void HighlightMission(ApartmentMissionCardView apartmentMissionCardView);
        void UnhighlightMission(ApartmentMissionCardView apartmentMissionCardView);
    }
}