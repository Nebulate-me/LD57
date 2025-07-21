using System.Collections.Generic;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomCardView : MonoBehaviour, IPointerClickHandler, IPoolableResource
    {
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private RectTransform roomCellContainer;
        [SerializeField] private GameObject roomTileCellPrefab;
        [SerializeField] private float cellSize = 0.33f; 
        
        [Space] 
        [SerializeField] private Image cardBackgroundImage;
        [SerializeField] private Sprite deselectedCardBackgroundSprite;
        [SerializeField] private Sprite selectedCardBackgroundSprite;

        [Inject] private IHandManager _handManager;
        [Inject] private IPrefabPool _prefabPool;
        
        private RoomDto _roomDto;
        private readonly List<RoomTileCellView> _roomTilCellViews = new();
        public RoomDto RoomDto => _roomDto;


        public void SetUp(RoomDto roomDto)
        {
            _roomDto = roomDto;
            roomName.text = roomDto.Name;

            roomCellContainer.DestroyChildren();
            foreach (var roomTileCell in roomDto.Tiles)
            {
                var roomTileCellView = _prefabPool.Spawn(roomTileCellPrefab, roomCellContainer).GetComponent<RoomTileCellView>();
                roomTileCellView.SetUp(roomTileCell);
                roomTileCellView.transform.position = roomCellContainer.transform.position + roomTileCell.Position.ToVector3() * cellSize;
                _roomTilCellViews.Add(roomTileCellView);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _handManager.SelectRoomCard(_roomDto);
        }

        public void Select()
        {
            cardBackgroundImage.sprite = selectedCardBackgroundSprite;
        }
        public void Deselect()
        {
            cardBackgroundImage.sprite = deselectedCardBackgroundSprite;
        }

        public void OnDespawn()
        {
            Deselect();
            foreach (var cellView in _roomTilCellViews)
            {
                _prefabPool.Despawn(cellView.gameObject);
            }
            _roomTilCellViews.Clear();
        }

        public void OnSpawn()
        {
            Deselect();
        }
    }
}