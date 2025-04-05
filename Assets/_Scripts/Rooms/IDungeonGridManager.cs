using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonGridManager
    {
        IReadOnlyList<DungeonRoomView> Rooms { get; }
        Bounds GetRoomBounds();
    }
}