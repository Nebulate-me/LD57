using System;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;

namespace _Scripts.Mascot
{
    public enum MascotTutorialBuildingTargetType
    {
        None = 0,
        Floor = 1,
        SharedRooms = 2,
        AllWindows = 3,
        Space = 4,
        Doors = 5,
        IndividualWindows = 6
    }
    
    [Serializable]
    public class MascotTutorialBuildingTargetToGameObjectDictionary : 
        UnitySerializedDictionary<MascotTutorialBuildingTargetType, GameObject>
    {
    }
}