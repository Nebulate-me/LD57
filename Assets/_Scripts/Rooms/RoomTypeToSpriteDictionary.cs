using System;
using Plugins.Sirenix.Odin_Inspector.Modules;
using UnityEngine;

namespace _Scripts.Rooms
{
    [Serializable]
    internal class RoomTypeToSpriteDictionary : UnitySerializedDictionary<RoomType, Sprite>
    {
    }
}