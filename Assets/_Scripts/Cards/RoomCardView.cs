using System;
using System.Collections.Generic;
using _Scripts.Game.Timer;
using _Scripts.Rooms;
using _Scripts.RoomTiles;
using _Scripts.Utils;
using Signals;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using Utilities.Prefabs;
using Zenject;

namespace _Scripts.Cards
{
    public class RoomCardView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPoolableResource
    {
        [SerializeField] private TextMeshProUGUI roomName;
        [Header("Room Icons")]
        [SerializeField] private GameObject roomIconPrefab;
        [SerializeField] private RectTransform roomIconContainer;
        [Header("Room Cells")]
        [SerializeField] private RectTransform roomCellContainer;
        [SerializeField] private GameObject roomTileCellPrefab;
        [SerializeField] private float cellSize = 0.33f; 
        
        [Space] 
        [SerializeField] private Image cardBackgroundImage;
        [SerializeField] private Sprite deselectedCardBackgroundSprite;
        [SerializeField] private Sprite hoveredCardBackgroundSprite;
        [SerializeField] private Sprite selectedCardBackgroundSprite;

        [Inject] private IHandManager _handManager;
        [Inject] private IPrefabPool _prefabPool;

        [ShowInInspector, ReadOnly] private bool _isEnabled;
        [ShowInInspector, ReadOnly] private bool _isSelected;
        [ShowInInspector, ReadOnly] private bool _isHovered;
        
        private RoomDto _roomDto;
        private readonly List<RoomTileCellView> _roomTilCellViews = new();
        
        public RoomDto RoomDto => _roomDto;

        private void OnEnable()
        { 
            SignalsHub.AddListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.AddListener<GameTimerPausedSignal>(OnGameTimerPaused);   
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<GameTimerStartedSignal>(OnGameTimerStarted);
            SignalsHub.RemoveListener<GameTimerPausedSignal>(OnGameTimerPaused);
        }

        private void OnGameTimerPaused(GameTimerPausedSignal obj)
        {
            SetIsEnabled(false);
        }

        private void OnGameTimerStarted(GameTimerStartedSignal obj)
        {
            SetIsEnabled(true);
        }

        private void SetIsEnabled(bool isEnabled)
        {
            _isEnabled = isEnabled;
            
            if (!isEnabled)
            {
                // _handManager.DeselectRoomCard();
            }
        }

        public void SetUp(RoomDto roomDto)
        {
            _isEnabled = true;
            _roomDto = roomDto;
            roomName.text = roomDto.Name;
            
            roomIconContainer.DestroyChildren();
            foreach (var roomType in roomDto.RoomTypes)
            {
                if (roomType == RoomTypeExtensions.ConnectingRoomType) continue;
                var roomTypeIcon = _prefabPool.Spawn(roomIconPrefab, roomIconContainer).GetComponent<RoomTypeIconView>();
                roomTypeIcon.SetUp(roomType);
            }

            roomCellContainer.DestroyChildren();
            var roomCenter = roomDto.GetCenter().ToVector3();
            foreach (var roomTileCell in roomDto.Tiles)
            {
                var roomTileCellView = _prefabPool.Spawn(roomTileCellPrefab, roomCellContainer).GetComponent<RoomTileCellView>();
                roomTileCellView.SetUp(roomTileCell);
                roomTileCellView.transform.position = roomCellContainer.transform.position +
                                                      (roomTileCell.Position.ToVector3() - roomCenter)* cellSize;
                _roomTilCellViews.Add(roomTileCellView);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isEnabled) return;
            _handManager.SelectRoomCard(_roomDto);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isEnabled) return;
            _isHovered = true;
            UpdateCardBackground();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isEnabled) return;
            _isHovered = false;
            UpdateCardBackground();
        }

        public void Select()
        {
            _isSelected = true;
            UpdateCardBackground();
        }
        public void Deselect()
        {
            _isSelected = false;
            UpdateCardBackground();
        }

        private void UpdateCardBackground()
        {
            cardBackgroundImage.sprite = _isSelected 
                ? selectedCardBackgroundSprite 
                : _isHovered 
                    ? hoveredCardBackgroundSprite
                    : deselectedCardBackgroundSprite;
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