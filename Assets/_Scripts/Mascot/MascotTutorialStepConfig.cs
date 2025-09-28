using System;
using _Scripts.Rooms;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Mascot
{
    [Serializable]
    public class MascotTutorialStepConfig
    {
        [SerializeField, TextArea(2, 5)] private string phrase = string.Empty;
        [SerializeField] private MascotTutorialTargetType targetType;
        [ShowIf(nameof(IsTargetTopPanel)), SerializeField] private MascotTutorialTopPanelTargetType topPanelTargetType;
        
        [ShowIf(nameof(IsTargetBottomPanel)), SerializeField] private MascotTutorialBottomPanelTargetType bottomPanelTargetType;
        [ShowIf(nameof(IsBottomPanelTargetTypeRoomCard)), SerializeField] private Room bottomPanelTargetRoom;
        
        [ShowIf(nameof(IsTargetBuilding)), SerializeField] private MascotTutorialBuildingTargetType buildingTargetType;
        [ShowIf(nameof(IsBuildingTargetTypeSpace)), SerializeField] private Vector2Int bottomLeftTargetCoordinate;
        [ShowIf(nameof(IsBuildingTargetTypeSpace)), SerializeField] private Vector2Int topRightTargetCoordinate;

        [SerializeField] private MascotTutorialActionType actionType;
        [ShowIf(nameof(IsActionTypeRoomCardSelectedOrPlaced)), SerializeField] private Room selectedRoomCard;
        [ShowIf(nameof(IsActionTypeRoomPlaced)), SerializeField] private Vector2 roomPosition;
        
        private bool IsTargetTopPanel => targetType == MascotTutorialTargetType.TopPanel;
        private bool IsTargetBottomPanel => targetType == MascotTutorialTargetType.BottomPanel;
        private bool IsTargetBuilding => targetType == MascotTutorialTargetType.Building;
        private bool IsBottomPanelTargetTypeRoomCard => bottomPanelTargetType == MascotTutorialBottomPanelTargetType.RoomCard;
        private bool IsBuildingTargetTypeSpace => IsTargetBuilding && buildingTargetType == MascotTutorialBuildingTargetType.Space;
        private bool IsActionTypeRoomCardSelectedOrPlaced => IsActionTypeRoomCardSelected || IsActionTypeRoomPlaced;
        private bool IsActionTypeRoomCardSelected => actionType == MascotTutorialActionType.RoomCardSelected;
        private bool IsActionTypeRoomPlaced => actionType == MascotTutorialActionType.RoomPlaced;
        
        public string Phrase => phrase;
        public MascotTutorialTargetType TargetType => targetType;
        public MascotTutorialTopPanelTargetType TopPanelTargetType => topPanelTargetType;
        public MascotTutorialBottomPanelTargetType BottomPanelTargetType => bottomPanelTargetType;
        public Room BottomPanelTargetRoom => bottomPanelTargetRoom;
        public MascotTutorialBuildingTargetType BuildingPanelTargetType => buildingTargetType;
        public MascotTutorialActionType ActionType => actionType;
        public Room SelectedRoomCard => selectedRoomCard;
    }
}