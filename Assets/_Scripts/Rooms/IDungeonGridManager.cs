using System.Collections.Generic;

namespace _Scripts.Rooms
{
    public interface IDungeonGridManager
    {
        IReadOnlyList<DungeonRoomView> Rooms { get; }
    }
}