using System.Collections.Generic;
using _Scripts.Rooms;

namespace _Scripts.Cards
{
    public interface IDeckManager
    {
        bool TryDraw(out RoomDto dto);
        void Bury(List<RoomDto> cardsToBury);
    }
}