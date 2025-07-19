using System.Collections.Generic;
using _Scripts.Rooms;
using _Scripts.RoomTiles;

namespace _Scripts.Cards
{
    public interface IDeckManager
    {
        int CardAmount { get; }
        bool TryDraw(out RoomTileDto tileDto);
        void Bury(List<RoomTileDto> cardsToBury);
    }
}