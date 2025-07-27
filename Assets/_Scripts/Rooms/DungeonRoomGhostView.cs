using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Rooms
{
    public class DungeonRoomGhostView : MonoBehaviour
    {
        [SerializeField] private Color validPlacementColor;
        [SerializeField] private Color invalidPlacementColor;
        [SerializeField] private Transform ghostTileContainer;
        [SerializeField] private GameObject ghostTileSprite;

        [Inject] private IPrefabPool _prefabPool;

        private RoomDto _currentRoomDto;
        private RoomDirection _currentRoomDirection = RoomDirection.North;
        private List<DungeonRoomTileGhostView> _roomTileGhostViews = new();
        
        public void SetUpValid(RoomDto roomDto, RoomDirection roomDirection)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto || _currentRoomDirection != roomDirection)
            {
                ResetCurrentRoomDto(roomDto, roomDirection);
            }
            
            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = validPlacementColor;
            }
        }

        public void SetUpInvalid(RoomDto roomDto, RoomDirection roomDirection)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto || _currentRoomDirection != roomDirection)
            {
                ResetCurrentRoomDto(roomDto, roomDirection);
            }
            
            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = invalidPlacementColor;
            }
        }
        
        private void ResetCurrentRoomDto(RoomDto roomDto, RoomDirection roomDirection)
        {
            if (_currentRoomDto != null)
            {
                foreach (var roomTileRenderer in _roomTileGhostViews)
                {
                    _prefabPool.Despawn(roomTileRenderer.gameObject);
                }
                _roomTileGhostViews = new List<DungeonRoomTileGhostView>();
                _currentRoomDto = null;
            }

            _currentRoomDto = roomDto;
            _currentRoomDirection = roomDirection;

            var rotatedRoomDto = _currentRoomDto.Rotate(roomDirection); 

            var roomTileStartingPosition = rotatedRoomDto.StartingPosition; 
            foreach (var roomTileCell in rotatedRoomDto.Tiles)
            {
                var tileGhostVew = _prefabPool.Spawn(ghostTileSprite, ghostTileContainer).GetComponent<DungeonRoomTileGhostView>();
                tileGhostVew.transform.localPosition = (roomTileCell.Position - roomTileStartingPosition).ToVector3();
                tileGhostVew.SetUp(roomTileCell);

                _roomTileGhostViews.Add(tileGhostVew);
            }
        }
    }
}