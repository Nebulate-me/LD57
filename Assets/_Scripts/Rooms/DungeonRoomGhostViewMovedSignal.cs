namespace _Scripts.Rooms
{
    public readonly struct DungeonRoomGhostViewMovedSignal
    {
        public RoomDto RoomDto { get; }
        public bool IsValid { get; }

        public DungeonRoomGhostViewMovedSignal(RoomDto roomDto, bool isValid)
        {
            RoomDto = roomDto;
            IsValid = isValid;
        }
    }
}