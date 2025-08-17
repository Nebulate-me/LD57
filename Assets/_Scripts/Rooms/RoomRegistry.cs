using System.Collections.Generic;
using System.Linq;
using _Scripts.Utils;
using ModestTree;
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
        
        public Sprite UnusedRoomFloorSprite => floorColorSprites.TryGetValue(unusedRoomFloorColor, out var roomFloorSprite) ? roomFloorSprite : null;
        public Sprite SharedRoomFloorSprite => floorColorSprites.TryGetValue(sharedRoomFloorColor, out var roomFloorSprite) ? roomFloorSprite : null;
        public RoomFloorColor UnusedRoomColor => unusedRoomFloorColor;
        public RoomFloorColor HighlightColor => highlightFloorColor;
        
        public Sprite GetRoomTypeIcon(RoomType roomType)
        {
            return roomTypeSprites.TryGetValue(roomType, out var roomIcon) ? roomIcon : defaultRoomTypeIcon;
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