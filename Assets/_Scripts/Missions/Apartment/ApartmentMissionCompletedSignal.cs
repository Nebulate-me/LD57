using System.Collections.Generic;
using _Scripts.Rooms;

namespace _Scripts.Missions.Apartment
{
    public struct ApartmentMissionCompletedSignal
    {
        public ApartmentMissionDto Dto { get; }
        public int Score { get; }
        public List<DungeonRoomModel> RoomsToUse { get; }
        
        public ApartmentMissionCompletedSignal(
            ApartmentMissionDto apartmentMissionDto, 
            int score,
            List<DungeonRoomModel> roomsToUse)
        {
            Dto = apartmentMissionDto;
            Score = score;
            RoomsToUse =  roomsToUse;
        }
    }
}