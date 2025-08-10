using System;
using _Scripts.Rooms;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.RoomTiles
{
    public class RoomTileCellView : MonoBehaviour
    {
        [FormerlySerializedAs("tileTransform")] [SerializeField] private RectTransform wallTileTransform;
        [FormerlySerializedAs("tileImage")] [SerializeField] private Image wallTileImage;
        [SerializeField] private RectTransform furnitureTransform;
        [SerializeField] private Image furnitureTileImage;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;


        public void SetUp(RoomTileCellDto roomTileCell)
        {
            wallTileImage.sprite = roomTileCell.Tile.UnusedSprite;
            wallTileTransform.rotation = roomTileCell.Direction.ToRotation();
            
            furnitureTileImage.gameObject.SetActive(roomTileCell.FurnitureSprite != null);
            furnitureTransform.rotation = roomTileCell.Direction.Rotate(roomTileCell.FurnitureDirection).ToRotation();
            furnitureTileImage.sprite = roomTileCell.FurnitureSprite;

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