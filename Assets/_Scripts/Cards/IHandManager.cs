using _Scripts.Rooms;
using _Scripts.RoomTiles;
using Utilities.Monads;

namespace _Scripts.Cards
{
    public interface IHandManager
    {
        int CardAmount { get; }
        IMaybe<RoomCardView> SelectedRoomCardView { get; }
        bool SelectRoomCard(RoomTileDto tileDto);
        bool DeselectRoomCard();
        bool TryPlaySelectRoomCard();
        void RefillHand();
    }
}