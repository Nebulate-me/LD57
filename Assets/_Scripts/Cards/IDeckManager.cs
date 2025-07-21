using System.Collections.Generic;
using _Scripts.Rooms;
using _Scripts.RoomTiles;

namespace _Scripts.Cards
{
    public interface IDeckManager
    {
        int RoomTileCardAmount { get; }
        bool TryDrawRoomTile(out RoomTileDto tileDto);
        void BuryRoomTile(List<RoomTileDto> cardsToBury);
        bool TryDrawRoom(out RoomDto roomDto);
        void BuryRoom(List<RoomDto> cardsToBury);
    }
}