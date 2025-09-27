using _Scripts.Rooms;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Achievements
{
    [CreateAssetMenu(menuName = "LD57/Create Achievement", fileName = "Achievement", order = 5)]
    public class Achievement : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string title;           
        [TextArea, SerializeField] private string description;
        [TextArea, SerializeField] private string conditionText;
        [SerializeField] private int points;
        
        [Header("Conditions")]
        [SerializeField] private AchievementType achievementType;
        
        [FormerlySerializedAs("apartmentTypeCount")] [SerializeField, ShowIf(nameof(IsApartmentTypeCountAchievementType))] private int apartmentRoomCountType = 0;
        [SerializeField, ShowIf(nameof(IsApartmentTypeCountAchievementType))] private int requiredCompletedApartments = 1;

        [SerializeField, ShowIf(nameof(IsRoomTypeCountOrWindowCountAchievementType))]
        private RoomType roomType = RoomType.Hallway;
        [SerializeField, ShowIf(nameof(IsRoomTypeCountAchievementType))] 
        private int requiredRooms = 1;
        [FormerlySerializedAs("requiredWindows")] [SerializeField, ShowIf(nameof(IsRoomTypeWindowCountAchievementType))] 
        private bool withWindows = false;
        
        public string Id => name;
        public string Title => title;
        public string Description => description;
        public string ConditionText => conditionText;
        public int Points => points;
        public AchievementType AchievementType => achievementType;
        public bool IsApartmentTypeCountAchievementType => achievementType == AchievementType.ApartmentTypeCount;
        public int ApartmentRoomCountType => apartmentRoomCountType;
        public int RequiredCompletedApartments => requiredCompletedApartments;
        public bool IsRoomTypeCountOrWindowCountAchievementType => IsRoomTypeCountAchievementType || IsRoomTypeWindowCountAchievementType;
        public bool IsRoomTypeCountAchievementType => achievementType == AchievementType.RoomTypeCount;
        public RoomType RoomType => roomType;
        public int RequiredRooms => requiredRooms;
        public bool IsRoomTypeWindowCountAchievementType => achievementType == AchievementType.RoomTypeWindowCount;
        public bool WithWindows => withWindows;
    }
}