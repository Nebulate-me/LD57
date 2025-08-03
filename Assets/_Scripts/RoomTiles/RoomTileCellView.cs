using System;
using _Scripts.Rooms;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.RoomTiles
{
    public class RoomTileCellView : MonoBehaviour
    {
        [SerializeField] private RectTransform tileTransform;
        [SerializeField] private Image tileImage;
        [SerializeField] private RoomDirectionToGameObjectDictionary doorObjects;
        public void SetUp(RoomTileCellDto roomTileCell)
        {
            tileImage.sprite = roomTileCell.Tile.UnusedSprite;
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