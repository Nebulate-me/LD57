using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Rooms;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace _Scripts.Missions
{
    public class MissionPatternCellView : MonoBehaviour
    {
        [SerializeField] private Image cellImage;

        [Space] [SerializeField] private Sprite roomCellSprite;
        [SerializeField] private Sprite emptyCellSprite;

        [Space] [SerializeField] private Sprite openConnectionSprite;
        [SerializeField] private Sprite closedConnectionSprite;

        [Space]
        [SerializeField] private List<DirectionImage> directionImages = new();

        public void SetUp(MissionCell patternCell)
        {
            switch (patternCell.Type)
            {
                case MissionCellType.Any:
                    return;
                case MissionCellType.Room:
                    cellImage.sprite = roomCellSprite;
                    break;
                case MissionCellType.Empty:
                    cellImage.sprite = emptyCellSprite;
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(patternCell.Type), patternCell.Type, null);
            }

            foreach (var roomDirection in EnumExtensions.GetAllItems<RoomDirection>())
            {
                var directionImage = directionImages.First(d => d.Direction == roomDirection);
                if (patternCell.OpenDirections.Contains(roomDirection))
                {
                    directionImage.Image.gameObject.SetActive(true);
                    directionImage.Image.sprite = openConnectionSprite;
                }
                else if (patternCell.ClosedDirections.Contains(roomDirection))
                {
                    directionImage.Image.gameObject.SetActive(true);
                    directionImage.Image.sprite = closedConnectionSprite;
                }
                else
                {
                    directionImage.Image.gameObject.SetActive(false);
                }
            }
        }
    }

    [Serializable]
    public class DirectionImage
    {
        [SerializeField] public RoomDirection Direction;
        [SerializeField] public Image Image;
    }
}