using System.Collections.Generic;

namespace _Scripts.Cards
{
    public interface IDeckManager
    {
        bool TryDraw(out RoomCard card);
        void Bury(List<RoomCard> cardsToBury);
    }
}