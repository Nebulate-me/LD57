using _Scripts.Rooms;
using Utilities.Monads;

namespace _Scripts.Cards
{
    public interface IHandManager
    {
        int CardAmount { get; }
        IMaybe<RoomCardView> SelectedRoomCardView { get; }
        bool SelectRoomCard(RoomDto dto);
        bool DeselectRoomCard();
        bool TryPlaySelectRoomCard();
        void RefillHand();
    }
}