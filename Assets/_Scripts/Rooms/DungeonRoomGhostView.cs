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
        private List<DungeonRoomTileGhostView> _roomTileGhostViews = new();
        
        public void SetUpValid(RoomDto roomDto)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto)
            {
                ResetCurrentRoomDto(roomDto);
            }
            
            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = validPlacementColor;
            }
        }

        public void SetUpInvalid(RoomDto roomDto)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto)
            {
                ResetCurrentRoomDto(roomDto);
            }
            
            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = invalidPlacementColor;
            }
        }
        
        private void ResetCurrentRoomDto(RoomDto roomDto)
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

            var roomTileStartingPosition = roomDto.StartingPosition; 
            foreach (var roomTileCell in _currentRoomDto.Tiles)
            {
                var tileGhostVew = _prefabPool.Spawn(ghostTileSprite, ghostTileContainer).GetComponent<DungeonRoomTileGhostView>();
                tileGhostVew.transform.localPosition = (roomTileCell.Position - roomTileStartingPosition).ToVector3();
                tileGhostVew.SetUp(roomTileCell);
                // TODO: consider rotation of the whole ghost room, with positions and rotations shifted

                _roomTileGhostViews.Add(tileGhostVew);
            }
        }
    }
}