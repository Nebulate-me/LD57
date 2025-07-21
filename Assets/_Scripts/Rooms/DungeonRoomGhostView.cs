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
        private List<SpriteRenderer> _roomTileRenderers = new();
        
        public void SetUpValid(RoomDto roomDto)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto)
            {
                ResetCurrentRoomDto(roomDto);
            }
            
            foreach (var roomTileRenderer in _roomTileRenderers)
            {
                roomTileRenderer.color = validPlacementColor;
            }
        }

        public void SetUpInvalid(RoomDto roomDto)
        {
            if (roomDto == null) return; // erroneous case
            if (_currentRoomDto != roomDto)
            {
                ResetCurrentRoomDto(roomDto);
            }
            
            foreach (var roomTileRenderer in _roomTileRenderers)
            {
                roomTileRenderer.color = invalidPlacementColor;
            }
        }
        
        private void ResetCurrentRoomDto(RoomDto roomDto)
        {
            if (_currentRoomDto != null)
            {
                foreach (var roomTileRenderer in _roomTileRenderers)
                {
                    _prefabPool.Despawn(roomTileRenderer.gameObject);
                }
                _roomTileRenderers = new List<SpriteRenderer>();
                _currentRoomDto = null;
            }

            _currentRoomDto = roomDto;

            var roomTileStartingPosition = roomDto.StartingPosition; 
            foreach (var roomTileCell in _currentRoomDto.Tiles)
            {
                var tileRenderer = _prefabPool.Spawn(ghostTileSprite, ghostTileContainer).GetComponent<SpriteRenderer>();
                tileRenderer.transform.localPosition = (roomTileCell.Position - roomTileStartingPosition).ToVector3();
                tileRenderer.sprite = roomTileCell.Tile.UnusedSprite;
                tileRenderer.transform.rotation = roomTileCell.Direction.ToRotation();
                // TODO: consider rotation of the whole ghost room, with positions and rotations shifted

                _roomTileRenderers.Add(tileRenderer);
            }
        }
    }
}