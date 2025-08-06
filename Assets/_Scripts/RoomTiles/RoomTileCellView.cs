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
        [SerializeField] private RectTransform tileTransform;
        [FormerlySerializedAs("tileImage")] [SerializeField] private Image wallTileImage;
        [SerializeField] private Image furnitureTileImage;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;
        

        public void SetUp(RoomTileCellDto roomTileCell)
        {
            wallTileImage.sprite = roomTileCell.Tile.UnusedSprite;
            furnitureTileImage.gameObject.SetActive(roomTileCell.FurnitureSprite != null);
            furnitureTileImage.sprite = roomTileCell.FurnitureSprite;
            tileTransform.rotation = roomTileCell.Direction.ToRotation();
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