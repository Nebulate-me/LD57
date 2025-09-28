using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Popups.LevelFinished;
using _Scripts.Utils;
using ModestTree;
using Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.RandomService;
using Zenject;

namespace _Scripts.Rooms
{
    public class RoomRegistry : MonoBehaviour, IRoomRegistry
    {
        [SerializeField] private Sprite defaultRoomTypeIcon;
        [SerializeField] private RoomTypeToSpriteDictionary roomTypeSprites;
        [Header("Floor")]
        [SerializeField] private RoomFloorColor unusedRoomFloorColor = RoomFloorColor.Gray;
        [SerializeField] private RoomFloorColor sharedRoomFloorColor = RoomFloorColor.Black;
        [SerializeField] private RoomFloorColor highlightFloorColor = RoomFloorColor.Yellow;
        [SerializeField] private FloorColorToSpriteDictionary floorColorSprites;

        [ShowInInspector, ReadOnly] private List<RoomFloorColor> _usedFloorColors = new();

        [Inject] private IRandomService _randomService;
        
        public Sprite UnusedRoomFloorSprite => floorColorSprites.GetValueOrDefault(unusedRoomFloorColor);
        public Sprite SharedRoomFloorSprite => floorColorSprites.GetValueOrDefault(sharedRoomFloorColor);
        public RoomFloorColor UnusedRoomColor => unusedRoomFloorColor;
        public RoomFloorColor HighlightColor => highlightFloorColor;

        private void OnEnable()
        {
            SignalsHub.AddListener<StartNextLevelSignal>(OnStartNextLevelSignal);
        }

        private void OnDisable()
        {
            SignalsHub.RemoveListener<StartNextLevelSignal>(OnStartNextLevelSignal);
        }

        private void OnStartNextLevelSignal(StartNextLevelSignal signal)
        {
            _usedFloorColors.Clear();
        }

        public Sprite GetRoomTypeIcon(RoomType roomType)
        {
            return roomTypeSprites.GetValueOrDefault(roomType, defaultRoomTypeIcon);
        }

        public Sprite GetRoomFloorSprite(RoomFloorColor color)
        {
            return floorColorSprites.TryGetValue(color, out var roomFloorSprite)
                ? roomFloorSprite
                : UnusedRoomFloorSprite;
        }

        public RoomFloorColor TakeUnusedColor()
        {
            if (floorColorSprites.IsEmpty()) return RoomFloorColor.Black;
            
            var unusedColors = floorColorSprites.Keys
                .Where(color => color != unusedRoomFloorColor && 
                                color != sharedRoomFloorColor &&
                                color != highlightFloorColor &&
                                !_usedFloorColors.Contains(color))
                .ToList();
            if (unusedColors.IsEmpty())
            {
                _usedFloorColors = new List<RoomFloorColor>();
                return TakeUnusedColor();
            }

            _randomService.ShuffleInPlace(unusedColors);
            var colorToUse = unusedColors.First();
            _usedFloorColors.Add(colorToUse);
            
            return colorToUse;
        }
    }
}