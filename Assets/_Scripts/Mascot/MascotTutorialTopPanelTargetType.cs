using System;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;

namespace _Scripts.Mascot
{
    public enum MascotTutorialTopPanelTargetType
    {
        None = 0,
        Timer = 1,
        MissionCards = 2,
        MissionCardName = 3,
        MissionCardRoomIcons = 4,
        MissionCardRoomWindows = 5,
        MissionCardScore = 6,
        Score = 7,
        MissionCounter = 8,
        UndoButton = 9,
        SingleMissionCard = 10
    }

    [Serializable]
    public class MascotTutorialTopPanelTargetToGameObjectDictionary : 
        UnitySerializedDictionary<MascotTutorialTopPanelTargetType, GameObject>
    {
    }
}