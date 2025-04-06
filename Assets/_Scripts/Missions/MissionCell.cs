using System;
using System.Collections.Generic;
using _Scripts.Rooms;
using UnityEngine;

namespace _Scripts.Missions
{
    [Serializable]
    public class MissionCell
    {
        [SerializeField] private MissionCellType type;
        [SerializeField] private Vector2Int position;
        [SerializeField] private List<RoomDirection> openDirections;
        [SerializeField] private List<RoomDirection> closedDirections;

        public MissionCell(
            MissionCellType cellType,
            Vector2Int cellPosition,
            List<RoomDirection> cellOpenDirections,
            List<RoomDirection> cellClosedDirections)
        {
            type = cellType;
            position = cellPosition;
            openDirections = cellOpenDirections;
            closedDirections = cellClosedDirections;
        }

        public MissionCellType Type => type;
        public Vector2Int Position => position;
        public List<RoomDirection> OpenDirections => openDirections;
        public List<RoomDirection> ClosedDirections => openDirections;
    }
}