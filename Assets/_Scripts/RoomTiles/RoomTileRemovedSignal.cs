using _Scripts.Rooms;

namespace _Scripts.RoomTiles
{
    public readonly struct RoomTileRemovedSignal
    {
        public DungeonRoomTileView RoomTile { get; }
        public RoomTileRemovedSignal(DungeonRoomTileView dungeonRoom)
        {
            RoomTile = dungeonRoom;
        }
    }
}