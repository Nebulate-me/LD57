namespace _Scripts.Rooms
{
    public readonly struct RoomPlacedSignal
    {
        private readonly DungeonRoomView room;

        public RoomPlacedSignal(DungeonRoomView dungeonRoom)
        {
            room = dungeonRoom;
        }

        public DungeonRoomView Room => room;
    }
}