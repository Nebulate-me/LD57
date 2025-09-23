namespace _Scripts.Rooms
{
    public readonly struct RoomCardSelectedSignal
    {
        public RoomDto RoomCard { get; }
        
        public RoomCardSelectedSignal(RoomDto roomCard)
        {
            RoomCard = roomCard;
        }
    }
}