using Utilities.Monads;

namespace _Scripts.Cards
{
    public interface IHandManager
    {
        IMaybe<RoomCardView> SelectedRoomCardView { get; }
        bool SelectRoomCard(RoomCard card);
        bool DeselectRoomCard();
        bool TryPlaySelectRoomCard();
        void RefillHand();
    }
}