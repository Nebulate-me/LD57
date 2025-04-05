using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Missions
{
    public class MissionPatternCellView: MonoBehaviour
    {
        [SerializeField] private Image cellImage;

        [Space]
        [SerializeField] private Sprite roomCellSprite;
        [SerializeField] private Sprite emptyCellSprite;

        public void SetUp(MissionCell patternCell)
        {
            switch (patternCell.Type)
            {
                case MissionCellType.Any:
                    break;
                case MissionCellType.Room:
                    cellImage.sprite = roomCellSprite;
                    break;
                case MissionCellType.Empty:
                    cellImage.sprite = emptyCellSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(patternCell.Type), patternCell.Type, null);
            }
        }
    }
}