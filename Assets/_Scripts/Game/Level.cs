using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Game
{
    [CreateAssetMenu(menuName = "LD57/Create Level", fileName = "Level", order = 1)]
    public class Level : ScriptableObject
    {
        [SerializeField] private Room startingRoom;
        [SerializeField] private Vector2Int startingPosition;
        
        public Room StartingRoom => startingRoom;
        public Vector2Int StartingPosition => startingPosition;
    }
}