namespace _Scripts.Rooms
{
    public readonly struct RoomRemovedSignal
    {
        public DungeonRoomModel Room { get; }
        
        public RoomRemovedSignal(DungeonRoomModel roomModel)
        {
            Room = roomModel;
        }
    }
}