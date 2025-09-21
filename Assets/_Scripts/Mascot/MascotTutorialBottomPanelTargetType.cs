using System;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;

namespace _Scripts.Mascot
{
    public enum MascotTutorialBottomPanelTargetType
    {
        None = 0,
        RoomCardsPanel = 1,
        RedrawCardsButton = 2,
        RoomCard = 3,
    }
    
    [Serializable]
    public class MascotTutorialBottomPanelTargetToGameObjectDictionary : 
        UnitySerializedDictionary<MascotTutorialBottomPanelTargetType, GameObject>
    {
    }
}