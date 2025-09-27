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
        [FormerlySerializedAs("apartmentTypeCount")] [SerializeField] private int apartmentRoomCountType = 0;
        [SerializeField] private int requiredCompletedApartments = 1;

        public string Id => name;
        public string Title => title;
        public string Description => description;
        public string ConditionText => conditionText;
        public int Points => points;
        public AchievementType AchievementType => achievementType;
        public int ApartmentRoomCountType => apartmentRoomCountType;
        public int RequiredCompletedApartments => requiredCompletedApartments;
    }
}