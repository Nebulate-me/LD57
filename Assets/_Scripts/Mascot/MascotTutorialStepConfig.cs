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

        [SerializeField] private MascotTutorialActionType actionType;
        
        private bool IsTargetTopPanel => targetType == MascotTutorialTargetType.TopPanel;
        private bool IsTargetBottomPanel => targetType == MascotTutorialTargetType.BottomPanel;
        private bool IsTargetBuilding => targetType == MascotTutorialTargetType.Building;
        private bool IsBottomPanelTargetTypeRoomCard => bottomPanelTargetType == MascotTutorialBottomPanelTargetType.RoomCard;
        
        public string Phrase => phrase;
        public MascotTutorialTargetType TargetType => targetType;
        public MascotTutorialTopPanelTargetType TopPanelTargetType => topPanelTargetType;
        public MascotTutorialBottomPanelTargetType BottomPanelTargetType => bottomPanelTargetType;
        public MascotTutorialBuildingTargetType BuildingPanelTargetType => buildingTargetType;
        public Room BottomPanelTargetRoom => bottomPanelTargetRoom;
        public MascotTutorialActionType ActionType => actionType;
        
    }
}