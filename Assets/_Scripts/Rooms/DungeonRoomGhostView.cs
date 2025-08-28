using System.Collections.Generic;
using _Scripts.Utils;
using Signals;
using Sirenix.OdinInspector;
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
        [Inject] private IDungeonGridManager _dungeonGridManager;

        private RoomDto _currentRoomDto;
        private List<DungeonRoomTileGhostView> _roomTileGhostViews = new();

        public void SetUpValid(RoomDto roomDto, RoomDirection roomDirection)
        {
            if (roomDto == null) return; // erroneous case
            ResetCurrentRoomDto(roomDto, roomDirection, isValid: true);

            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = validPlacementColor;
            }
        }

        public void SetUpInvalid(RoomDto roomDto, RoomDirection roomDirection)
        {
            if (roomDto == null) return; // erroneous case
            
            ResetCurrentRoomDto(roomDto, roomDirection, isValid: false);
            
            
            foreach (var tileGhostView in _roomTileGhostViews)
            {
                tileGhostView.Color = invalidPlacementColor;
            }
        }
        
        private void ResetCurrentRoomDto(RoomDto roomDto, RoomDirection roomDirection, bool isValid)
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

            var rotatedRoomDto = _currentRoomDto.Rotate(roomDirection); 

            var roomTileStartingPosition = rotatedRoomDto.StartingPosition; 
            foreach (var roomTileCell in rotatedRoomDto.Tiles)
            {
                var tileGhostVew = _prefabPool.Spawn(ghostTileSprite, ghostTileContainer).GetComponent<DungeonRoomTileGhostView>();
                tileGhostVew.transform.localPosition = (roomTileCell.Position - roomTileStartingPosition).ToVector3(); // FIXME: gridToWorld this?
                tileGhostVew.SetUp(roomTileCell, roomDto);

                _roomTileGhostViews.Add(tileGhostVew);
            }

            var positionShift = _dungeonGridManager.WorldToGrid(transform.position.ToVector2Int()) - roomTileStartingPosition;
            SignalsHub.DispatchAsync(new DungeonRoomGhostViewMovedSignal(rotatedRoomDto.Shift(positionShift), isValid));
        }
    }
}