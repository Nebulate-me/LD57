using _Scripts.Rooms;

namespace _Scripts.RoomTiles
{
    public readonly struct RoomTilePlacedSignal
    {
        private readonly DungeonRoomView room;

        public RoomTilePlacedSignal(DungeonRoomView dungeonRoom)
        {
            room = dungeonRoom;
        }

        public DungeonRoomView Room => room;
    }
}