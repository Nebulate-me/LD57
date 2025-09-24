using _Scripts.Rooms;

namespace _Scripts.RoomTiles
{
    public readonly struct RoomTileRemovedSignal
    {
        private readonly DungeonRoomTileView room;

        public RoomTileRemovedSignal(DungeonRoomTileView dungeonRoom)
        {
            room = dungeonRoom;
        }

        public DungeonRoomTileView Room => room;
    }
}