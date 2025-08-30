using System.Collections.Generic;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;

namespace _Scripts.Missions
{
    public interface IMissionManager
    {
        int CompletableMissionsCount { get; }
        List<RoomType> UnfulfilledRoomTypeRequirements { get; }
        void CompleteMission(ApartmentMissionCardView apartmentMissionCard);
        void HighlightMission(ApartmentMissionCardView apartmentMissionCardView);
        void UnhighlightMission(ApartmentMissionCardView apartmentMissionCardView);
    }
}