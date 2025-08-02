namespace _Scripts.Rooms
{
    public class RoomPlacedSignal
    {
        public DungeonRoomModel Room { get; }
        public RoomPlacedSignal(DungeonRoomModel roomModel)
        {
            Room = roomModel;
        }
    }
}