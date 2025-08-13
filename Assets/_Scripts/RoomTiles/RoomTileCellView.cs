using System;
using _Scripts.Rooms;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.RoomTiles
{
    public class RoomTileCellView : MonoBehaviour
    {
        [FormerlySerializedAs("tileTransform")] [SerializeField] private RectTransform wallTileTransform;
        [FormerlySerializedAs("tileImage")] [SerializeField] private Image wallTileImage;
        [Space]
        [SerializeField] private RectTransform furnitureTransform;
        [SerializeField] private Image furnitureTileImage;
        [Space] 
        [SerializeField] private Image floorTileImage;
        [Space]
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;

        [Inject] private IRoomRegistry _roomRegistry;
        
        public void SetUp(RoomTileCellDto roomTileCell)
        {
            wallTileImage.sprite = roomTileCell.Tile.UnusedSprite;
            wallTileTransform.rotation = roomTileCell.Direction.ToRotation();
            
            furnitureTileImage.gameObject.SetActive(roomTileCell.FurnitureSprite != null);
            furnitureTransform.rotation = roomTileCell.Direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();
            furnitureTileImage.sprite = roomTileCell.FurnitureSprite;

            floorTileImage.sprite = _roomRegistry.UnusedRoomFloorSprite;

            foreach (var doorObject in doorObjects.Values)
            {
                doorObject.SetActive(false);
            }

            var rotatedDoorDirections = roomTileCell.Tile.DoorDirections.Rotate(roomTileCell.Direction);
            foreach (var tileDoorDirection in rotatedDoorDirections)
            {
                doorObjects[tileDoorDirection].SetActive(true);
            }
        }
    }


    [Serializable]
    public class RoomDirectionToGameObjectDictionary : UnitySerializedDictionary<RoomDirection, GameObject>
    {
        
    }
}