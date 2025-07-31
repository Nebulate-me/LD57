using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Rooms
{
    public interface IDungeonGridManager
    {
        IReadOnlyList<DungeonRoomTileView> Rooms { get; }
        Bounds GetRoomBounds();
    }
}