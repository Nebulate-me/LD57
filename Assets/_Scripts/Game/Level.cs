using System.Collections.Generic;
using _Scripts.Missions.Apartment;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(menuName = "LD57/Create Level", fileName = "Level", order = 1)]
    public class Level : ScriptableObject
    {
        [SerializeField] private string levelName;
        [SerializeField] private Vector2Int levelSize = new(20, 15);
        [SerializeField] private List<RoomPositionDto> startingPlacedRooms = new();
        
        [Header("Room Cards")]
        [SerializeField] private List<Room> availableRooms = new();
        [SerializeField] private List<Room> initialRooms = new();
        [SerializeField] private int initialRemainingRoomCards;
        
        [Header("Missions")]
        [SerializeField] private List<ApartmentMission> availableMissions = new();
        [SerializeField] private List<ApartmentMission> initialMissions = new();
        [SerializeField] private int missionsToComplete = 4;

        public List<RoomPositionDto> StartingPlacedRooms => startingPlacedRooms;

        /// <summary>
        /// The real size in cells the Level would take <br/>
        /// 10 = 10 / 2 * 2 + 1 => 11 <br/>
        /// 11 = 11 / 2 * 2 + 1 => 11 <br/>
        /// 12 = 12 / 2 * 2 + 1 => 12 <br/>
        /// etc.
        /// </summary>
        public Vector2Int LevelSize => HalfLevelSize * 2 + Vector2Int.one;
        public Vector2Int HalfLevelSize => levelSize / 2;

        public bool Contains(Vector2Int gridPosition)
        {
            return gridPosition.x >= -HalfLevelSize.x && gridPosition.x <= HalfLevelSize.x &&
                   gridPosition.y >= -HalfLevelSize.y && gridPosition.y <= HalfLevelSize.y;
        }
        
        public IList<Room> AvailableRooms => availableRooms;
        public IReadOnlyList<Room> InitialRooms => initialRooms;
        public int InitialRemainingRoomCards => initialRemainingRoomCards; 
        public IReadOnlyList<ApartmentMission> AvailableMissions => availableMissions;
        public IReadOnlyList<ApartmentMission> InitialMissions => initialMissions;
        public int MissionsToComplete => missionsToComplete;
    }
}