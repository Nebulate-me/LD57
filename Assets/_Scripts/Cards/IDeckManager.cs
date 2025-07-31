using System.Collections.Generic;
using _Scripts.Rooms;

namespace _Scripts.Cards
{
    public interface IDeckManager
    {
        int RoomCardAmount { get; }
        bool TryDrawRoom(out RoomDto roomDto);
        void BuryRoom(List<RoomDto> cardsToBury);
    }
}