using _Scripts.Rooms;

namespace _Scripts.RoomTiles
{
    public readonly struct RoomTilePlacedSignal
    {
        private readonly DungeonRoomTileView room;

        public RoomTilePlacedSignal(DungeonRoomTileView dungeonRoom)
        {
            room = dungeonRoom;
        }

        public DungeonRoomTileView Room => room;
    }
}