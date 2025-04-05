using System;
using UnityEngine;

namespace _Scripts.Missions
{
    [Serializable]
    public class MissionCell
    {
        [SerializeField] private MissionCellType type = MissionCellType.Room;
        [SerializeField] private Vector2Int position;

        public MissionCell(MissionCellType cellType, Vector2Int cellPosition)
        {
            type = cellType;
            position = cellPosition;
        }

        public MissionCellType Type => type;
        public Vector2Int Position => position;
    }
}